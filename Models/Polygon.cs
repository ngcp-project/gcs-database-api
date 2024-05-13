namespace Database.Models
{

    public class Polygon : Shape
    {
        Coordinate[] points;
        public Polygon(string name, Coordinate[] points) : base(name, ShapeType.Polygon)
        {
            this.points = points;
        }
    }

}