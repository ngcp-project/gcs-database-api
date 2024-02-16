using System.Net.WebSockets;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using StackExchange.Redis;
using RabbitMQ.Client.Events;
using Database.Handlers;

namespace Database.Controllers
{
    public class WebSocketController : ControllerBase
    {
        private readonly ILogger<WebSocketController> _logger;
        private readonly IDatabase _redis;
        private readonly RabbitMqConsumer _rabbitmq;

        public WebSocketController(
            ILogger<WebSocketController> logger, 
            IConnectionMultiplexer redis,
            RabbitMqConsumer rabbitmq)
        {
            _logger = logger;
            _redis = redis.GetDatabase();
            _rabbitmq = rabbitmq;
        }


        [HttpGet("/ws/{vehicleName}")]
        public async Task GetTelemetry(string vehicleName)
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                using WebSocket ws = await HttpContext.WebSockets.AcceptWebSocketAsync();
                string queueName = $"telemetry_{vehicleName.ToLower()}";

                _logger.Log(LogLevel.Information, "\nWebsocket connection established!\n");
                _logger.Log(LogLevel.Information, "\nQueue: " + queueName);
                _rabbitmq.QueueDeclare(queueName);
                _rabbitmq.StartConsuming(queueName);
                // Tie a callback function with type `EventHandler<BasicDeliveryEventArgs> to the consumer`                
                _rabbitmq.Consumer.Received += async (channel, eventArgs) => 
                {
                    var body = eventArgs.Body.ToString();

                    await ws.SendAsync(
                        new ArraySegment<byte>(eventArgs.Body.ToArray()),
                        WebSocketMessageType.Text,
                        true,
                        CancellationToken.None
                    );
                };

                // Stop listening to the specific client when specified
                while (true) 
                {
                    if (ws.State == WebSocketState.Open)
                    {
                        Thread.Sleep(100);
                    }
                    else if (ws.State == WebSocketState.Closed || ws.State == WebSocketState.Aborted)
                    {
                        _logger.Log(LogLevel.Error, "Websocket connection aborted!");
                        _rabbitmq.StopListening();
                        break;
                    }

                }
            }
            else HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        }

    }
}