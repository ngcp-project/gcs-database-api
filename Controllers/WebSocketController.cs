using System;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Database.Models;

using StackExchange.Redis;

namespace Database.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class WebSocketController : ControllerBase
    {
        private new const int BadRequest = ((int)HttpStatusCode.BadRequest);
        private readonly ILogger<WebSocketController> _logger;

        private ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost");
        private IDatabase db;

        public WebSocketController(ILogger<WebSocketController> logger)
        {
            _logger = logger;
            db = redis.GetDatabase();
        }

        [Route("/ws")]
        public async Task Get()
        {

            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                _logger.Log(LogLevel.Information, "WebSocket connection established");
                await Echo(webSocket);
            }
            else {
                HttpContext.Response.StatusCode = BadRequest;
            }
        }
        
        private async Task Echo(WebSocket webSocket)
        {
            var buffer = new byte[1024 * 4];
            var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            _logger.Log(LogLevel.Information, "Message received from Client");

            while (!result.CloseStatus.HasValue)
            {
                //string value = await db.StringGetAsync("foo");
                //Console.WriteLine(value);
                var inputString = Encoding.UTF8.GetString(buffer);
                var vehicleData = JsonSerializer.Deserialize<BaseTelemetry>(inputString);
                var vehicleId = vehicleData.Id;

                //var serverMsg = Encoding.UTF8.GetBytes($"Server: Hello. You said: {Encoding.UTF8.GetString(buffer)}");
                await webSocket.SendAsync(new ArraySegment<byte>(buffer, 0, buffer.Length), result.MessageType, result.EndOfMessage, CancellationToken.None);
                // _logger.Log(LogLevel.Information, "Message sent to Client");

                db.StringSet(vehicleData?.Name, inputString);

                buffer = new byte[1024 * 4];
                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                _logger.Log(LogLevel.Information, "Message received from Client");
            }

            await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
            _logger.Log(LogLevel.Information, "WebSocket connection closed");
        }
    }
}