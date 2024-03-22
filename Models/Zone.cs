

using System.Reflection.Metadata;

public enum ShapeType { Polygon, Circle };

namespace Database.Models
{
    public class Zone
    {
        public string name { get; set;}
        public bool? keepIn {get; set;}
        public ShapeType shapeType {get; set;}
        public Coordinate[] coordinates {get; set;}

        public Zone(string name, string shapeType, Coordinate[] coordinates) {
            this.name = name;
            keepIn = default;
            
            if (shapeType.ToLower() == "polygon") {
                this.shapeType = ShapeType.Polygon;
            } else if (shapeType.ToLower() == "circle") {
                this.shapeType = ShapeType.Circle;
            }

            this.coordinates = coordinates;
        }

    }
}