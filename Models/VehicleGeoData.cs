using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Database.Models;

namespace Database.Models
{
    public class VehicleGeoData
    {

        public Coordinate target { get; set; }
        public Shape searchArea { get; set; }
        public string localIP { get; set; }

    }
}