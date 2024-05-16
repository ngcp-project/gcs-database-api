using System.Net.WebSockets;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using StackExchange.Redis;
using RabbitMQ.Client.Events;
using Database.Handlers;
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

        // static readonly ConcurrentDictionary<string, ArrayList> _vehicleConnIds = new ConcurrentDictionary<string, ArrayList>();
        // static readonly ConcurrentDictionary<string, WebSocket> _wsConnections = new ConcurrentDictionary<string, WebSocket>();

        static readonly ConcurrentDictionary<string, List<WebSocket>> _wsConnections = new ConcurrentDictionary<string, List<WebSocket>>();

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
                string connKey = Interlocked.Increment(ref connId).ToString();

                // 
                bool newConsumer = true;

                // Group WebSocket connections by vehicle 
                _wsConnections.AddOrUpdate(vehicleName.ToLower(), new List<WebSocket>() { ws }, (key, oldValue) =>
                {
                    newConsumer = false;
                    oldValue.Add(ws);
                    return oldValue;
                });

                _logger.Log(LogLevel.Information, $"\nWebsocket connection established with {vehicleName.ToUpper()}{connKey}!\n");

                // Use a CancellationTokenSource to control the loop
                CancellationTokenSource tokenSource = null;

                // Start RabbitMQ consumer if it is the first WebSocket connection
                if (newConsumer)
                {
                    string queueName = $"telemetry_{vehicleName.ToLower()}";
                    _logger.Log(LogLevel.Information, "\nQueue: " + queueName);
                    _rabbitmq.CreateConsumer(queueName);

                    tokenSource = new();

                    // Start the RabbitMQ consumer in a separate task
                    Task task = Task.Run(() => ListenToRabbitMq(ws, queueName, tokenSource.Token, vehicleName));
                }

                byte[] buffer = new byte[1024 * 4];
                WebSocketReceiveResult result = null;

                // Stop listening to the specific client when specified
                do
                {
                    try
                    {
                        result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                    }
                    catch (WebSocketException e)
                    {
                        break;
                    }
                    Console.Write("-");
                    Thread.Sleep(100);

                } while (!result.CloseStatus.HasValue);

                // if (tokenSource != null)
                // {
                // tokenSource.Cancel();
                // _rabbitmq.StopConsuming();
                // }

                // _logger.Log(LogLevel.Error, "ABORTING WEBSOCKET CONNECTION FOR " + vehicleName.ToUpper() + connKey + "!\n");
                CloseWebSocketConnection(vehicleName, ws, _logger, connKey);

                // _logger.Log(LogLevel.Error, "Websocket connection aborted!");
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

                _wsConnections.TryGetValue(vehicleName, out List<WebSocket> wsList);

                // Send vehicle data to all relevant WebSocket connections
                foreach (WebSocket wsConn in wsList)
                {
                    await wsConn.SendAsync(
                        new ArraySegment<byte>(eventArgs.Body.ToArray()),
                        WebSocketMessageType.Text,
                        true,
                        CancellationToken.None
                    );
                    // }
                }
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


        // **NOTE: Not sure if lock is needed. Seems to work without it
        // private static SemaphoreSlim _lock = new SemaphoreSlim(1);

        private static void CloseWebSocketConnection(string vehicleName, WebSocket ws, ILogger<WebSocketController> _logger, string connKey)
        {
            // _lock.Wait();
            // try
            // {
            // Remove WebSocket connection from list
            _wsConnections.TryGetValue(vehicleName.ToLower(), out List<WebSocket> wsList);
            wsList.Remove(ws);
            _logger.Log(LogLevel.Error, "Websocket connection closed for " + vehicleName.ToUpper() + connKey + "!\n");

            // Close the WebSocket connection
            ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "Connection closed", CancellationToken.None);
            // }
            // finally
            // {
            // _lock.Release();
            // }
        }
    }
}