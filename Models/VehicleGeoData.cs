using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Database.Models;

namespace Database.Models
{
    public class VehicleGeoData
    {
        public bool IsManual { get; set; } = true;

        public Coordinate Target { get; set; }

        //need shape class
        public int SearchArea { get; set; }
    }
}