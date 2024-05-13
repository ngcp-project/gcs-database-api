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
        public double lastUpdated { get; set; }
        public Coordinate currentPosition { get; set; }
        [optional]
        public bool fireFound { get; set; }
        [optional]
        public Coordinate fireCoordinate { get; set; }
    }
}
