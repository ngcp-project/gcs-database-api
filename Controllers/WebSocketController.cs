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
using System.Collections;
using System.Configuration;

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

        static readonly ConcurrentDictionary<string, ArrayList> _vehicleConnIds = new ConcurrentDictionary<string, ArrayList>();
        static readonly ConcurrentDictionary<string, WebSocket> _wsConnections = new ConcurrentDictionary<string, WebSocket>();

        static int connId = 0;

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
                // _numVehicleConnections.AddOrUpdate(vehicleName.ToLower(), 1, (key, oldValue) => oldValue + 1);
                // _vehicleConnIds.AddOrUpdate(vehicleName.ToLower(), new ArrayList(), (key, oldValue) => oldValue.Add(connId++);
                string key = connId.ToString();
                _vehicleConnIds.AddOrUpdate(vehicleName.ToLower(), new ArrayList(), (key, oldValue) => { oldValue.Add(connId++); return oldValue; });

                // Create unique key for WebSocket connection
                string wsKey = $"{vehicleName.ToLower()}_{key}";

                // Add WebSocket connection to dictionary
                _wsConnections.TryAdd(wsKey, ws);

                _logger.Log(LogLevel.Information, $"\nWebsocket connection established with {vehicleName.ToUpper()}!\n");

                // Use a CancellationTokenSource to control the loop
                CancellationTokenSource tokenSource = null;

                // Start RabbitMQ consumer if it is the first WebSocket connection
                if (_vehicleConnIds[vehicleName.ToLower()].Count == 1)
                {
                    string queueName = $"telemetry_{vehicleName.ToLower()}";
                    _logger.Log(LogLevel.Information, "\nQueue: " + queueName);
                    _rabbitmq.CreateConsumer(queueName);

                    tokenSource = new();

                    // Start the RabbitMQ consumer in a separate task
                    Task task = Task.Run(() => ListenToRabbitMq(ws, queueName, tokenSource.Token, vehicleName));
                }

                byte[] buffer = new byte[1024 * 4];
                WebSocketReceiveResult result;

                // Stop listening to the specific client when specified
                do
                {
                    result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                    Console.Write("-");
                    Thread.Sleep(100);

                } while (!result.CloseStatus.HasValue);

                if (tokenSource != null)
                {
                    tokenSource.Cancel();
                    _rabbitmq.StopConsuming();
                }

                // Remove connection from dictionary when WebSocket connection is closed
                WebSocket ows;
                _wsConnections.TryRemove(key, out ows);
                // {
                //     if (ows != ws)
                //     {
                //         // Add back the connection if it was removed too fast
                //         _wsConnections.AddOrUpdate(key, ows, (p, w) => ows);
                //     }
                // }

                _logger.Log(LogLevel.Error, "Websocket connection aborted!");
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
        private async Task ListenToRabbitMq(WebSocket ws, string queueName, CancellationToken cancellationToken, string vehicleName)
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
                var vehicleKey = vehicleName;

                // Save vehicle data to database
                _logger.Log(LogLevel.Information, "Saving vehicle data to database!");
                _redis.StringSet(vehicleKey, inputString);

                _logger.Log(LogLevel.Information, "Sending WebSocket messages...");

                _vehicleConnIds.TryGetValue(queueName.Split("_")[1], out ArrayList connIds);
                foreach (string id in connIds)
                {
                    _logger.Log(LogLevel.Information, "i");
                    if (_wsConnections.ContainsKey($"{queueName}_{id}"))
                    {
                        await _wsConnections[$"{queueName}_{id}"].SendAsync(
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