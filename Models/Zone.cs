public enum ShapeType { Polygon, Circle };

namespace Database.Models
{

    public class Zone
    {
        public string name { get; set; }
        public bool? keepIn { get; set; }
        public string shapeType { get; set; }
        public Coordinate[] coordinates { get; set; }

        public Zone(string name, string shapeType, Coordinate[] coordinates)
        {
            this.name = name;
            keepIn = default;
            this.shapeType = shapeType;
            // if (String.Equals(shapeType.ToLower(),"polygon")) {
            //     Console.WriteLine("polygon");
            //     this.shapeType = ShapeType.polygon;
            // } else if (String.Equals(shapeType.ToLower(),"circle")) {
            //     Console.WriteLine(" circle");
            //     this.shapeType = ShapeType.circle;
            // }

            this.coordinates = coordinates;
        }

    }
}