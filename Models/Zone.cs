using System.Text.Json;

namespace Database.Models
{

    public class Zone
    {
        public bool keepIn { get; set; }
        public Coordinate[] coordinates { get; set; }

        public Zone(Coordinate[] coordinates)
        {
            keepIn = false;

            this.coordinates = coordinates;
        }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }

    }
}