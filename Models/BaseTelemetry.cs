using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Database.Models
{
    public class BaseTelemetry
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Coordinate Coordinate { get; set; }
        public double Speed { get; set; }
        public double BatteryLife { get; set; }
    }
}