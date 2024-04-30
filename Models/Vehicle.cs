namespace Database.Models
{
    public class Vehicle
    {
        public string key { get; set; }
        public float speed { get; set; }
        public float yaw { get; set; }
        public float roll { get; set; }
        public float altitude { get; set; }
        public float batteryLife { get; set; }
        public string lastUpdated { get; set; }
        public Coordinate currentPosition { get; set; }
        public Status_Enum vehicleStatus { get; set; }
    }
}