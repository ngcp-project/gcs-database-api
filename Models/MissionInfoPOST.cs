namespace Database.Models
{
    public class MissionInfoPOST
    {

        public string missionName { get; set; }
        public string stageName { get; set; }
        public VehicleData[] vehicleKeys { get; set; }
    }
}