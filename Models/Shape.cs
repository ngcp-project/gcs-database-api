using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public enum ShapeType { Polygon, Circle};

namespace Database.Models
{
    public abstract class Shape{
        private string name; 
        private ShapeType shapeType;

        public Shape (string shapeName, ShapeType type){
            if (string.IsNullOrWhiteSpace(shapeName)){
                throw new ArgumentException("The shape name is required");
            }
            name = shapeName;

            shapeType = type;
        }
    }
}