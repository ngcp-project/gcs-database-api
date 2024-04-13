using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Database.Models{
    public class MissionStages
    {
        public string Key {get; set;}
        public string stageName {get; set;}
        public Stage_Enum stageStatus {get; set;}

        // public VehicleGeoData[] vehicleKeys {get; set;}
    }
}