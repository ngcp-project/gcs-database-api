using System.Net.WebSockets;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using StackExchange.Redis;
using RabbitMQ.Client.Events;
using Database.Handlers;
using System.IO;
using System.Text.Json;
using Database.Models;
using System.Collections.Concurrent;

namespace Database.Controllers
{
    /**
    * <summary>
    *   Receives vehicle data from vehicles via RabbitMQ, saves vehicle data to database, and sends
    *   data to front end via a WebSocket connection.
    *   <para />
    *   Note: A WebSocket connection must be established before establishing a RabbitMQ connection. 
    * </summary>
    */
    public class WebSocketController : ControllerBase
    {
        private readonly ILogger<WebSocketController> _logger;
        private readonly IDatabase _redis;
        private readonly RabbitMqConsumer _rabbitmq;

        static readonly ConcurrentDictionary<string, int> _numVehicleConnections = new ConcurrentDictionary<string, int>();
        static readonly ConcurrentDictionary<string, WebSocket> _wsConnections = new ConcurrentDictionary<string, WebSocket>();

        public WebSocketController(
            ILogger<WebSocketController> logger,
            IConnectionMultiplexer redis,
            RabbitMqConsumer rabbitmq)
        {
            _logger = logger;
            _redis = redis.GetDatabase();
            _rabbitmq = rabbitmq;
        }

        /**
        * <summary>
        *   Establish a WebSocket connection to retrieve vehicle telemetry from backend
        *   <para />
        *   Note: A RabbitMQ connection will be established upon a WebSocket connection 
        * </summary>
        * <param name="vehicleName">Name of vehicle to retrieve telemetry data</param>
        */
        [HttpGet("/ws/{vehicleName}")]
        public async Task GetTelemetry(string vehicleName)
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                // Accept the WebSocket connection & establish queue name
                using WebSocket ws = await HttpContext.WebSockets.AcceptWebSocketAsync();

                // Update number of connections for given vehicle
                _numVehicleConnections.AddOrUpdate(vehicleName.ToLower(), 1, (key, oldValue) => oldValue + 1);

                // Create unique key for WebSocket connection
                string wsKey = $"{vehicleName.ToLower()}_{_numVehicleConnections[vehicleName.ToLower()]}";

                // Add WebSocket connection to dictionary
                _wsConnections.TryAdd(wsKey, ws);

                _logger.Log(LogLevel.Information, $"\nWebsocket connection established with {vehicleName.ToUpper()}!\n");

                if (_numVehicleConnections[vehicleName.ToLower()] == 1)
                {
                    string queueName = $"telemetry_{vehicleName.ToLower()}";
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
            }
            else HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        }

        /**
                * <summary>
                *   Establish a WebSocket connection to retrieve vehicle telemetry from backend
                * </summary>
                * <param name="ws">Open WebSocket connection</param>
                * <param name="queueName">RabbitMQ consumer queueName. Format: telemetry_vehiclename (all lowercase)</param>
                * <param name="cancellationToken">Token used to cancel or dispose of RabbitMQ consumer</param>
                */
        private async Task ListenToRabbitMq(WebSocket ws, string queueName, CancellationToken cancellationToken)
        {
            // Add a callback function to RabbitMQ
            // NOTE: This will be triggered whenever a RabbitMQ message is received
            _rabbitmq.Consumer.Received += async (channel, eventArgs) =>
            {
                var body = eventArgs.Body.ToArray();
                var inputString = Encoding.UTF8.GetString(body).TrimEnd('\0');
                // Deserialize JSON String
                Vehicle vehicleData = new Vehicle();

                _logger.Log(LogLevel.Information, inputString);

                try
                {
                    vehicleData = JsonSerializer.Deserialize<Vehicle>(inputString);
                }
                catch (Exception e)
                {
                    _logger.Log(LogLevel.Error, "Error: " + e.Message);
                    return;
                }

                // Get Vehicle key
                var vehicleKey = vehicleData.key;

                // Save vehicle data to database
                _logger.Log(LogLevel.Information, "Saving vehicle data to database!");
                _redis.StringSet(vehicleKey, inputString);

                _logger.Log(LogLevel.Information, "Sending WebSocket messages...");

                for (int i = 0; i < _numVehicleConnections[queueName]; i++)
                {
                    _logger.Log(LogLevel.Information, "i");
                    if (_wsConnections.ContainsKey($"{queueName}_{i}"))
                    {
                        await _wsConnections[$"{queueName}_{i}"].SendAsync(
                            new ArraySegment<byte>(eventArgs.Body.ToArray()),
                            WebSocketMessageType.Text,
                            true,
                            CancellationToken.None
                        );
                    }
                }

                // await ws.SendAsync(
                //     new ArraySegment<byte>(eventArgs.Body.ToArray()),
                //     WebSocketMessageType.Text,
                //     true,
                //     CancellationToken.None
                // );
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