namespace Database.Models
{
    public class MissionInfoGET
    {

        public string missionName { get; set; }
        public string stageName { get; set; }
        public VehicleData[] vehicleKeys { get; set; }
    }
}