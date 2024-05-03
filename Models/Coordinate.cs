namespace Database.Models
{
    public class Coordinate
    {
        public double latitude {get; set;}
        public double longitude {get; set;}

        public Coordinate(double latitude, double longitude) {
            this.latitude = latitude;
            this.longitude = longitude;
        }
    }
}