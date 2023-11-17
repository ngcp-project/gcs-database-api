using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Database.Models{

    public class Polygon : Shape
    {
        Coordinate[] points;
        public Polygon(string name, Coordinate[] points) : base (name, ShapeType.Polygon) {
            this.points = points;
        }
    }

}