using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace Database.Models
{
    public class Coordinate
    {
        public double latitude {get; set;}
        public double longitude {get; set;}

        // private double latitude;
        // private double longitude;

        // public Coordinate(double latitude, double longitude) {
        //     this.latitude = latitude;
        //     this.longitude = longitude;
        // }

        // public double getLatitude() {
        //     return latitude;
        // }

        // public void setLatitude(double latitude) {
        //     this.latitude = latitude;
        // }

        // public double getLongitude() {
        //     return longitude;
        // }

        // public void setLongitude(double longitude) {
        //     this.longitude = longitude;
        // }
    }
}