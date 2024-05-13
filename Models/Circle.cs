namespace Database.Models
{

    public class Circle : Shape
    {
        double radius;
        Coordinate center;
        public Circle(string name, Coordinate center, double radius) : base(name, ShapeType.Circle)
        {
            this.radius = radius;
            this.center = center;
        }
    }
}