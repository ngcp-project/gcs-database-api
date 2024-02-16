using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Database.Models;

namespace Database.Models
{
    public class VehicleGelocation
    {
        public bool IsManual { get; set; } = true;

        public Coordinate Target { get; set; }

        //need shape class
        public int SearchArea { get; set; }
    }
}