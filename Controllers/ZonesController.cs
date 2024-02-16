using Microsoft.AspNetCore.Mvc;


    public class ZonesController : ControllerBase
    {
        [HttpGet("{keepIn}")]
        public String getZones(string keepIn) {
            if (keepIn.ToLower() == "in") {
                // return keep-in zones
                return "hello world";
            }

            // else return keep-out zones
            return "out";
        }
    }
