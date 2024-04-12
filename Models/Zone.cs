
using System.Security.Cryptography.X509Certificates;

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
            
            if (String.Equals(shapeType.ToLower(),"polygon")) {
                zoneShapeType = ShapeType.Polygon;
            } else if (String.Equals(shapeType.ToLower(),"circle")) {
                zoneShapeType = ShapeType.Circle;
            } else {
                zoneShapeType = ShapeType.Unknown;
            }

            this.coordinates = coordinates;
        }

        public override string ToString() {
            // stringify coordinates
            string coordinatesString = "\n[\n";
            int coordCount = this.coordinates.Length;
            for (int i = 0; i < coordCount; i++) {
                coordinatesString += $"{{\"latitude\": {this.coordinates[i].latitude}, \"longitude\": {this.coordinates[i].longitude}}}";
                if (i != coordCount - 1) {
                    coordinatesString += ",\n";
                } else {
                    coordinatesString += "\n]";
                }
            }

            // return data to post in json format
            return $"{{\n\"name\": {this.name},\n \"shapeType\": {this.shapeType},\n \"coordinates\": {coordinatesString}\n}}";
        }

    }
}