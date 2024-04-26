using System.Text.Json;


namespace Database.Models
{
    public class VehicleData
    {
        public String vehicleName { get; set; }
        public bool IsManual { get; set; } = true;
        public Coordinate Target { get; set; }
        public Coordinate[] SearchArea { get; set; }

        public String localIP { get; set; }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }

    }
}