using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace gcs-database-api.Models
{
    public class MissionStage
    {
        public string Key {get; set;}
        public string stageName {get; set;}
        public STAGE_ENUM stageStatus {get; set;}

        public VehicleGeoData[] vehicleKeys {get; set;}
    }
}