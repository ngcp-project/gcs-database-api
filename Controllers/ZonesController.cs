using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;

    public class ZonesController : ControllerBase
    {
        private ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost");
        private readonly IDatabase _redis;

        public ZonesController () {
            _redis = redis.GetDatabase();
        }

        [HttpGet("zones/{keepIn}")]
        public String getZones(string keepIn) {
            if (keepIn.ToLower() == "in") {
                // return keep-in zones
                return _redis.StringGet("keepIn");
            }

            // else return keep-out zones
            if (keepIn.ToLower() == "out") {
                // return keep-in zones
                return _redis.StringGet("keepOut");
            }

            return "invalid zone name";
        }


    }
