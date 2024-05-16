using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using System.Text.Json;
using Database.Models;
using System.Reflection;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

public class EmergencyStopController : ControllerBase
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
}