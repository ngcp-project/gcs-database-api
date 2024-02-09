using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;


    public class ZonesController : ControllerBase
    {
        private readonly IDatabase _redis;

        public ZonesController (
            IConnectionMultiplexer redis
        ) {
            _redis = redis.GetDatabase();
        }

        [HttpGet("{keepIn}")]
        public String getZones(string keepIn) {
            if (keepIn.ToLower() == "in") {
                // return keep-in zones
                return _redis.StringGet("keepIn");
            }

            // else return keep-out zones
            return _redis.StringGet("keepOut");
        }
    }
