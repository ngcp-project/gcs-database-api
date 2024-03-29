using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Database.Models
{
    public class VehicleStages
    {
        public string Key {get; set;}
        public int currentStageId {get; set;}
        public string stageName {get; set;}
        public int stageStatus {get; set;}
        
    }
}