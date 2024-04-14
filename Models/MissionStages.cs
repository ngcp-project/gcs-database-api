using System;
using System.Runtime.CompilerServices;


namespace Database.Models{
    public class MissionStages
    {
        // public MissionStages(string key, string stageName, Stage_Enum stageStatus){
        //     this.Key = key;
        //     this.stageName = stageName;
        //     this.stageStatus = stageStatus;
        // }


        public string key {get; set;}
        public string stageName {get; set;}
        public Stage_Enum stageStatus {get; set;}

        // public VehicleGeoData[] vehicleKeys {get; set;}
    }
}