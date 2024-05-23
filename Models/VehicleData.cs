using System.Text.Json;


namespace Database.Models
{
    public class VehicleData
    {
        public String vehicleName { get; set; }
        public bool IsManual { get; set; } = true;
        public Coordinate target { get; set; }
        public Coordinate[] searchArea { get; set; }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }

    }
}