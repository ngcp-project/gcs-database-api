using System;
using System.Runtime.CompilerServices;


namespace Database.Models{
    public class MissionStage
    {

        public string key {get; set;}
        public string stageId {get; set;}
        public string stageName {get; set;}
        public Stage_Enum stageStatus {get; set;}
        public VehicleGeoData[] vehicleKeys {get; set;}
    }
}