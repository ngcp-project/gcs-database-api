using System.Text.Json;

public enum ShapeType { Polygon, Circle, Unknown };

namespace Database.Models
{

    public class Zone
    {
        public string name { get; set; }
        public bool? keepIn { get; set; }
        public string shapeType { get; set; }
        public ShapeType? zoneShapeType { get; set; }
        public Coordinate[] coordinates { get; set; }

        public Zone(string name, string shapeType, Coordinate[] coordinates)
        {
            this.name = name;
            keepIn = false;
            this.shapeType = shapeType;

            if (String.Equals(shapeType.ToLower(), "polygon"))
            {
                zoneShapeType = ShapeType.Polygon;
            }
            else if (String.Equals(shapeType.ToLower(), "circle"))
            {
                zoneShapeType = ShapeType.Circle;
            }
            else
            {
                zoneShapeType = ShapeType.Unknown;
            }

            this.coordinates = coordinates;
        }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }

    }
}