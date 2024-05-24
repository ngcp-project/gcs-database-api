using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using Database.Models;
using System.Reflection;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

public class MissionStageController : ControllerBase
{
    private ConnectionMultiplexer conn;
    private readonly IDatabase gcs;

    public MissionStageController()
    {
        conn = DBConn.Instance().getConn();
        gcs = conn.GetDatabase();
    }


    [HttpGet("MissionStage")]
    public IActionResult GetMissionStage([FromQuery] MissionStageQuery requestBody)
    {
        List<string> missingFields = new List<string>();

        EndpointReturn endpointReturn = new EndpointReturn("", "", "");
        Type type = typeof(MissionStageQuery);
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

        if (gcs.StringGet(requestBody.missionName).IsNullOrEmpty)
        {
            endpointReturn.error = "Inputted MissionInfo does not exist";
            return BadRequest(endpointReturn.ToString());
        }

        MissionInfo missionInfo = JsonSerializer.Deserialize<MissionInfo>(gcs.StringGet(requestBody.missionName));
        List<MissionStage> stages = missionInfo.stages.ToList();

        bool foundMissionStage = false;
        foreach (MissionStage stage in stages)
        {
            if (stage.stageName == requestBody.stageName)
            {
                endpointReturn.data = JsonSerializer.Serialize(stage);
                endpointReturn.message = "Found MissionStage";
                foundMissionStage = true;
                break;
            }
        }

        if (!foundMissionStage)
        {
            endpointReturn.error = "Inputted MissionStage does not exist";
            return BadRequest(endpointReturn.ToString());
        }

        return Ok(endpointReturn.ToString());
    }


    [HttpPost("MissionStage")]
    public async Task<IActionResult> SetMissionStage([FromBody] MissionStagePOST requestBody)
    {
        List<string> missingFields = new List<string>();

        EndpointReturn endpointReturn = new EndpointReturn("", "", "");
        Type type = typeof(MissionStagePOST);
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

        if (gcs.StringGet(requestBody.missionName).IsNullOrEmpty)
        {
            endpointReturn.error = "Inputted MissionInfo does not exist";
            return BadRequest(endpointReturn.ToString());
        }


        MissionInfo missionInfo = JsonSerializer.Deserialize<MissionInfo>(gcs.StringGet(requestBody.missionName));
        missionInfo.stages = requestBody.stages;
        
        // RabbitMQ sender
        ConnectionFactory factory = new ConnectionFactory(){HostName = "localhost"};
        var cts = new CancellationTokenSource();
        MissionStage nextStageInfo = missionInfo.stages[missionInfo.currentStageId];
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
            
            // consumer event handler
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
                string target = JsonSerializer.Serialize(vehicle.target);
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
            doesVehiclesReply = await CurrentStageReplyTimer(cts.Token);
        }

        if (!doesVehiclesReply)
        {
            endpointReturn.message = "No vehicles replied to GCS!";
            return StatusCode(500, endpointReturn.ToString());
        }

        await gcs.StringSetAsync(requestBody.missionName, missionInfo.ToString());

        endpointReturn.message = "Posted MissionStage";
        return Ok(endpointReturn.ToString());
    }

    [HttpDelete("MissionStage")]
    public async Task<IActionResult> DeleteMissionStage([FromBody] MissionStageQuery requestBody)
    {
        List<string> missingFields = new List<string>();

        EndpointReturn endpointReturn = new EndpointReturn("", "", "");
        Type type = typeof(MissionStageQuery);
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


        if (gcs.StringGet(requestBody.missionName).IsNullOrEmpty)
        {
            endpointReturn.error = "Inputted MissionInfo does not exist";
            return BadRequest(endpointReturn.ToString());
        }
        // Scans for correct MissionStage entry

        MissionInfo missionInfo = JsonSerializer.Deserialize<MissionInfo>(gcs.StringGet(requestBody.missionName));
        List<MissionStage> stages = missionInfo.stages.ToList();

        bool foundMissionStage = false;
        foreach (MissionStage stage in stages)
        {
            // Handle deletion of current stage
            if (missionInfo.stages[missionInfo.currentStageId].stageName == requestBody.stageName)
            {
                missionInfo.currentStageId = 0;
            }
            if (stage.stageName == requestBody.stageName)
            {
                stages.Remove(stage);
                missionInfo.stages = stages.ToArray();
                await gcs.StringSetAsync(requestBody.missionName, missionInfo.ToString());
                endpointReturn.message = "Deleted MissionStage";
                foundMissionStage = true;
                break;
            }
        }

        if (!foundMissionStage)
        {
            endpointReturn.error = "Inputted MissionStage does not exist";
            return BadRequest(endpointReturn.ToString());
        }

        return Ok(endpointReturn.ToString());
    }

    private async Task<bool> CurrentStageReplyTimer(CancellationToken token)
    {
        try {

            await Task.Delay(3000, token);
        } 
        catch (TaskCanceledException) {
            return true;
        }

        return false;
    }

}