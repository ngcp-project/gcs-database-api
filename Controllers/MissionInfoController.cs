using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using Database.Models;
using System.Reflection;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Threading.Channels;


namespace Database.Controllers;
public class MissionInfoController : ControllerBase
{
    private ConnectionMultiplexer conn;
    private readonly IDatabase gcs;

    public MissionInfoController()
    {
        conn = DBConn.Instance().getConn();
        gcs = conn.GetDatabase();
    }

    [HttpGet("MissionInfo")]
    public IActionResult GetMissionInfo([FromQuery] MissionInfoGET requestBody)
    {

        List<string> missingFields = new List<string>();

        EndpointReturn endpointReturn = new EndpointReturn("", "", "");
        Type type = typeof(MissionInfoGET);
        PropertyInfo[] properties = type.GetProperties();

        foreach (System.Reflection.PropertyInfo property in properties)
        {
            var value = property.GetValue(requestBody, null);
            object defaultValue = null;
            if (property.PropertyType == typeof(string))
            {
                defaultValue = null;
            }

            else if (property.PropertyType.IsValueType)
            {
                defaultValue = Activator.CreateInstance(property.PropertyType);
            }

            if (value?.Equals(defaultValue) == true || value == null)
            {
                missingFields.Add(property.Name);
            }
            if (missingFields.Count > 0)
            {
                endpointReturn.error = "Missing fields: " + string.Join(", ", missingFields);
                return BadRequest(endpointReturn.ToString());
            }

        }
        string result = gcs.StringGet(requestBody.missionName).ToString();
        endpointReturn.data = result;
        return Ok(endpointReturn.ToString());
    }


    [HttpPost("MissionInfo")]
    public async Task<IActionResult> SetMissionInfo([FromBody] MissionInfoPOST requestBody)
    {
        List<string> missingFields = new List<string>();

        EndpointReturn endpointReturn = new EndpointReturn("", "", "");
        Type type = typeof(MissionInfoPOST);
        PropertyInfo[] properties = type.GetProperties();

        foreach (PropertyInfo property in properties)
        {
            var value = property.GetValue(requestBody, null);
            object defaultValue = null;
            if (property.PropertyType == typeof(string))
            {
                defaultValue = null;
            }

            else if (property.PropertyType.IsValueType)
            {
                defaultValue = Activator.CreateInstance(property.PropertyType);
            }

            if (value?.Equals(defaultValue) == true || value == null)
            {
                missingFields.Add(property.Name);
            }
            if (missingFields.Count > 0)
            {
                endpointReturn.error = "Missing fields: " + string.Join(", ", missingFields);
                return BadRequest(endpointReturn.ToString());
            }
        }

        List<MissionStage> stages = new List<MissionStage>();
        MissionStage initialStage = new MissionStage(requestBody.stageName, requestBody.vehicleKeys);
        stages.Add(initialStage);

        MissionInfo missionInfo = new MissionInfo
        {
            missionName = requestBody.missionName,
            // currentStageId = requestBody.stageName,
            // currentStageId = requestBody.stageName,
            stages = stages.ToArray()
        };
        // Initializes new MissionInfo object with a MissionStage attached to it


        await gcs.StringSetAsync(requestBody.missionName, missionInfo.ToString());
        endpointReturn.message = "Posted MissionInfo";
        return Ok(endpointReturn.ToString());
    }

    [HttpPost("CurrentStage")]
    public async Task<IActionResult> SetCurrentStage([FromBody] CurrentStagePOST requestBody)
    {
        List<string> missingFields = new List<string>();

        EndpointReturn endpointReturn = new EndpointReturn("", "", "");
        Type type = typeof(CurrentStagePOST);
        PropertyInfo[] properties = type.GetProperties();

        foreach (System.Reflection.PropertyInfo property in properties)
        {
            var value = property.GetValue(requestBody, null);
            object defaultValue = null;
            if (property.PropertyType == typeof(string))
            {
                defaultValue = null;
            }

            else if (property.PropertyType.IsValueType)
            {
                defaultValue = Activator.CreateInstance(property.PropertyType);
            }

            if (value?.Equals(defaultValue) == true || value == null)
            {
                missingFields.Add(property.Name);
            }
            if (missingFields.Count > 0)
            {
                endpointReturn.error = "Missing fields: " + string.Join(", ", missingFields);
                return BadRequest(endpointReturn.ToString());
            }
        }

        // Pull MissionInfo from database and update currentStageId
        try
        {
            MissionInfo missionInfo = JsonSerializer.Deserialize<MissionInfo>(gcs.StringGet(requestBody.missionName));

            if (missionInfo.currentStageId == missionInfo.stages.Length - 1)
            {
                endpointReturn.error = "Mission is already at the last stage";
                return BadRequest(endpointReturn.ToString());
            }

            // RabbitMQ sender
            int nextStage = missionInfo.currentStageId + 1;
            ConnectionFactory factory = new ConnectionFactory(){HostName = "localhost"};
            var cts = new CancellationTokenSource();
            MissionStage nextStageInfo = missionInfo.stages[nextStage];
            bool doesVehiclesReply = false;

            using (IConnection conn = factory.CreateConnection())
            using (IModel channel = conn.CreateModel())
            {
                var replyQueueName = channel.QueueDeclare().QueueName;
                var consumer = new EventingBasicConsumer(channel);
                var responseTask = new TaskCompletionSource<IActionResult>();
                var correlationId = Guid.NewGuid().ToString();
                var props = channel.CreateBasicProperties();
                props.CorrelationId = correlationId;
                props.ReplyTo = replyQueueName;

                consumer.Received += (model, ea) =>
                {
                    if (ea.BasicProperties.CorrelationId == correlationId)
                    {
                        var response = Encoding.UTF8.GetString(ea.Body.ToArray());
                        cts.Cancel();
                    }
                };


                // Interate through each vehicle and publish data to respective channels
                foreach (VehicleData vehicle in nextStageInfo.vehicleKeys)
                {
                    // target coordinate
                    string target = JsonSerializer.Serialize<Coordinate>(vehicle.target);
                    byte[] targetBytes = Encoding.UTF8.GetBytes(target);
                    // string targetReplyQueue = $"{replyQueueName}_{vehicle.vehicleName}_target";
                    // var targetProps = props;
                    // targetProps.ReplyTo = targetReplyQueue;

                    channel.BasicPublish(
                        exchange: "",
                        routingKey: $"{vehicle.vehicleName.ToLower()}_command_target",
                        basicProperties: props,
                        body: targetBytes
                    );
                    Console.WriteLine($"{vehicle.vehicleName.ToLower()}_command_target - {target}");
                    // channel.BasicConsume(consumer: consumer, queue: targetReplyQueue, autoAck: true);

                    // manual mode toggle
                    string manual = JsonSerializer.Serialize(new { isManual = vehicle.IsManual });
                    byte[] manualBytes = Encoding.UTF8.GetBytes(manual);
                    // string manualReplyQueue = $"{replyQueueName}_{vehicle.vehicleName}_manual";
                    // var manualProps = props;
                    // manualProps.ReplyTo = manualReplyQueue;
                    channel.BasicPublish(
                        exchange: "",
                        routingKey: $"{vehicle.vehicleName.ToLower()}_command_manual",
                        basicProperties: props,
                        body: manualBytes
                    );
                    Console.WriteLine($"{vehicle.vehicleName.ToLower()}_command_manual - {manual}");
                    // channel.BasicConsume(consumer: consumer, queue: manualReplyQueue, autoAck: true);

                    // search area
                    string searchArea = JsonSerializer.Serialize(vehicle.searchArea);
                    byte[] searchAreaBytes = Encoding.UTF8.GetBytes(searchArea);
                    // string searchReplyQueue = $"{replyQueueName}_{vehicle.vehicleName}_search";
                    // var searchProps = props;
                    // searchProps.ReplyTo = searchReplyQueue;
                    channel.BasicPublish(
                        exchange: "",
                        routingKey: $"{vehicle.vehicleName.ToLower()}_command_search",
                        basicProperties: props,
                        body: searchAreaBytes
                    );
                    Console.WriteLine($"{vehicle.vehicleName.ToLower()}_command_search - {searchArea}");
                    // channel.BasicConsume(consumer: consumer, queue: searchReplyQueue, autoAck: true);
                }
                channel.BasicConsume(consumer: consumer, queue: replyQueueName, autoAck: true);
                // Initiate the current stage reply timer to wait for reply
                doesVehiclesReply = await CurrentStageReplyTimer(cts.Token, nextStageInfo.vehicleKeys.Length);
            }


            // foreach (MissionStage stage in missionInfo.stages)
            // {
            //     if (stage.stageName == requestBody.currentStage)
            //     {
            //         missionInfo.currentStageId = requestBody.currentStage;
            //         await gcs.StringSetAsync(requestBody.missionName, missionInfo.ToString());

            //         endpointReturn.message = "Posted CurrentStage";
            //         return Ok(endpointReturn.ToString());
            //     }
            // }
            
            if (!doesVehiclesReply)
            {
                endpointReturn.error = "No replies received from the vehicles";
                return StatusCode(500, endpointReturn.ToString());
            }

            missionInfo.currentStageId = nextStage;
            
            await gcs.StringSetAsync(requestBody.missionName, missionInfo.ToString());
            
            endpointReturn.message = "Updated CurrentStage to " + missionInfo.stages[missionInfo.currentStageId].stageName;
            return Ok(endpointReturn.ToString());
        }
        catch (Exception e)
        {
            endpointReturn.error = e.Message;
            return BadRequest(endpointReturn.ToString());
        }


    }

    private async Task<bool> CurrentStageReplyTimer(CancellationToken token, int numOfVehicles)
    {
        // int maxReplyCount = numOfVehicles;
        // int replyCount = 0;
        // object lockObject = new object();

        // Task monitorReplies = Task.Run(() =>
        // {
        //     while (replyCount < maxReplyCount)
        //     {
        //         token.WaitHandle.WaitOne(); // Wait for a cancellation request (reply form vehicle)
        //         lock (lockObject)
        //         {
        //             if (token.IsCancellationRequested)
        //             {
        //                 replyCount++;
        //                 Console.WriteLine($"Received reply:{replyCount}");
        //                 if (replyCount < maxReplyCount)
        //                     token = new CancellationTokenSource().Token;
        //             }
        //         }
        //     }
        // });


        try {
            await Task.Delay(3000, token);
        }
        catch (TaskCanceledException) {
            
            return true;
        }

        return false;
    }
}
