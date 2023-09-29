using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Database.Models
{
    public class Geofence
    {
        private bool isKeepIn;
        private Shape shape;

        public void setShape(Shape shape) {
            this.shape = shape;
        }

        public Shape getShape() {
            return shape;
        }

        public void setKeepIn(bool isKeepIn) {
            this.isKeepIn = isKeepIn;
        }

        public bool getKeepIn() {
            return isKeepIn;
        }
    }
}