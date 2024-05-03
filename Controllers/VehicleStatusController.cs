using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using System.Text.Json;
using Database.Models;
using System.Reflection;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

public class VehicleStatusController : ControllerBase
{
    private ConnectionMultiplexer conn;
    private readonly IDatabase gcs;

    public VehicleStatusController()
    {
        conn = DBConn.Instance().getConn();
        gcs = conn.GetDatabase();
    }

    /*
    --------------Posts and Gets----------------
    Body format for all posts and gets in this file:
    {
        "Key": "[VehicleName]"
    }
    */

    [HttpPost("SetStatusInUse")]
    public async Task<IActionResult> SetStatusInUse([FromBody] VehicleKey requestBody)
    {
        List<string> missingFields = new List<string>();

        EndpointReturn endpointReturn = new EndpointReturn("", "", "");
        Type type = typeof(VehicleKey); // Replace ExampleModel with the respective model
        PropertyInfo[] properties = type.GetProperties();
        foreach (System.Reflection.PropertyInfo property in requestBody.GetType().GetProperties())
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

        }
        // Iterates through every property in the model and checks if it is null or default value

        if (missingFields.Count > 0)
        {
            endpointReturn.error = "Missing fields: " + string.Join(", ", missingFields);
            return BadRequest(endpointReturn.ToString());
        }
        // If any field is missing, return a bad request

        await gcs.StringSetAsync($"{requestBody.Key}-status", "1"); // Replace "example" with the respective database key
        endpointReturn.message = "In Use";
        return Ok(endpointReturn.ToString());
    }

    [HttpPost("SetStatusStandby")]
    public async Task<IActionResult> SetStatusStandby([FromBody] VehicleKey requestBody)
    {
        List<string> missingFields = new List<string>();

        EndpointReturn endpointReturn = new EndpointReturn("", "", "");
        Type type = typeof(VehicleKey); // Replace ExampleModel with the respective model
        PropertyInfo[] properties = type.GetProperties();
        foreach (System.Reflection.PropertyInfo property in requestBody.GetType().GetProperties())
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

        }
        // Iterates through every property in the model and checks if it is null or default value

        if (missingFields.Count > 0)
        {
            endpointReturn.error = "Missing fields: " + string.Join(", ", missingFields);
            return BadRequest(endpointReturn.ToString());
        }
        // If any field is missing, return a bad request

        await gcs.StringSetAsync($"{requestBody.Key}-status", "2"); // Replace "example" with the respective database key
        endpointReturn.message = "Standby";
        return Ok(endpointReturn.ToString());
    }

  [HttpPost("EmergencyStop")]
public async Task<IActionResult> EmergencyStop([FromBody] VehicleKey requestBody)
{
    List<string> missingFields = new List<string>();

    PropertyInfo[] properties = typeof(VehicleKey).GetProperties();
    foreach (PropertyInfo property in properties)
    {
        var value = property.GetValue(requestBody, null);
        object defaultValue = property.PropertyType.IsValueType ? Activator.CreateInstance(property.PropertyType) : null;

        if (value?.Equals(defaultValue) == true || value == null)
            missingFields.Add(property.Name);
    }

    if (missingFields.Count > 0)
        return BadRequest("Missing fields: " + string.Join(", ", missingFields));

    // RabbitMQ Part
    var factory = new ConnectionFactory() { HostName = "localhost" };
    using (var connection = factory.CreateConnection())
    using (var channel = connection.CreateModel())
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
                // Handle response here
                HttpContext.Items["RabbitMQResponse"] = response;
            }
        };

        string message = JsonSerializer.Serialize(new { command = "stop", vehicleKey = requestBody.Key });
        var messageBytes = Encoding.UTF8.GetBytes(message);
        channel.BasicPublish(exchange: "", routingKey: "rpc_queue", basicProperties: props, body: messageBytes);
        channel.BasicConsume(consumer: consumer, queue: replyQueueName, autoAck: true);

        _ = Task.Delay(3000).ContinueWith(task => responseTask.TrySetResult(BadRequest("No response from vehicle server.")));

        return await responseTask.Task;  // This will return as soon as the response is set or the timeout expires
    }
}


    [HttpGet("GetVehicleStatus")]
    public async Task<string> GetVehicleStatus()
    {
        int num = 0; //Default of 0, database value will be read into this variable
        using (var sr = new StreamReader(Request.Body))
        {
            string vehicleData = await sr.ReadToEndAsync();
            VehicleKey? vehicleKey = JsonSerializer.Deserialize<VehicleKey>(vehicleData);
            string key = $"{vehicleKey?.Key}-status";
            num = (int)await gcs.StringGetAsync(key);
        }
        string output; //output string, will be returned. 
        //Switch block to conert int value in database to string value for enum. 
        switch (num)
        {
            case 1:
                output = "In Use";
                break;
            case 2:
                output = "Standby";
                break;
            case 3:
                output = "Emergency Stopped";
                break;
            default:
                output = "Invalid";
                break;
        }
        return output;
    }
}