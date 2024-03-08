using System.Net.WebSockets;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using StackExchange.Redis;
using RabbitMQ.Client.Events;
using Database.Handlers;
using System.IO;

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
                // Accept the WebSocket connection & establish queue name
                using WebSocket ws = await HttpContext.WebSockets.AcceptWebSocketAsync();
                string queueName = $"telemetry_{vehicleName.ToLower()}";

                _logger.Log(LogLevel.Information, $"\nWebsocket connection established with {vehicleName.ToUpper()}!\n");
                _logger.Log(LogLevel.Information, "\nQueue: " + queueName);
                _rabbitmq.CreateConsumer(queueName);

                // Use a CancellationTokenSource to control the loop
                CancellationTokenSource tokenSource = new();

                // Start the RabbitMQ consumer in a separate task
                Task task = Task.Run(() => ListenToRabbitMq(ws, queueName, tokenSource.Token));

                byte[] buffer = new byte[1024 * 4];
                WebSocketReceiveResult result;

                // Stop listening to the specific client when specified
                do
                {
                    result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                    Console.Write("-");
                    Thread.Sleep(100);

                } while (!result.CloseStatus.HasValue);

                tokenSource.Cancel();

                _logger.Log(LogLevel.Error, "Websocket connection aborted!");
                _rabbitmq.StopConsuming();

            }
            else HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        }

        private async Task ListenToRabbitMq(WebSocket ws, string queueName, CancellationToken cancellationToken)
        {
            // Add a callback function to RabbitMQ
            // NOTE: This will be triggered whenever a RabbitMQ message is received
            _rabbitmq.Consumer.Received += async (channel, eventArgs) =>
            {
                var body = eventArgs.Body.ToString().TrimEnd('\0');
                // Deserialize JSON String

                Vehicle vehicleData = JsonSerializer.Deserialize<Vehicle>(inputString);

                // Get Vehicle key
                var vehicleKey = vehicleData.key;

                 _logger.Log(LogLevel.Information, "Sending WebSocket message...");
                // Save to database
                _redis.StringSet(vehicleKey, inputString);

                _logger.Log(LogLevel.Information, "Sending WebSocket message...");

                await ws.SendAsync(
                    new ArraySegment<byte>(eventArgs.Body.ToArray()),
                    WebSocketMessageType.Text,
                    true,
                    CancellationToken.None
                );
            };

            _rabbitmq.StartConsuming(queueName);

            try
            {
                // Wait for cancellation or dispose the consumer
                await Task.Delay(Timeout.Infinite, cancellationToken);
            }
            finally
            {
                _rabbitmq.StopConsuming();
            }
        }

    }
}