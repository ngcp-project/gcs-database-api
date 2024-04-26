using System;
using System.Runtime.CompilerServices;

namespace Database.Models
{
    public class MissionStage
    {
        public string key { get; set; }
        public string stageId { get; set; }
        public string stageName { get; set; }
        public Stage_Enum stageStatus { get; set; }
        public VehicleGeoData[] vehicleKeys { get; set; }

        public MissionStage(string key, string stageId, string stageName, string stageStatus, VehicleGeoData[] vehicleKeys)
        {
            this.key = key;
            this.stageId = stageId;
            this.stageName = stageName;

            switch (stageStatus.ToLower())
            {
                case "not_started":
                    this.stageStatus = Stage_Enum.NOT_STARTED;
                    break;
                case "initialized":
                    this.stageStatus = Stage_Enum.INITIALIZED;
                    break;
                case "in_progress":
                    this.stageStatus = Stage_Enum.IN_PROGRESS;
                    break;
                case "completed":
                    this.stageStatus = Stage_Enum.COMPLETE;
                    break;

            }

            this.vehicleKeys = vehicleKeys;
        }
    }


}
