using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Database.Models;


namespace Database.Models
{
    public class VehicleData
    {
        public bool IsManual { get; set; } = true;
        public Coordinate Target { get; set; }
        public Coordinate[] SearchArea { get; set; }

    public override string ToString() {
        string targetCoordStr = "\n[\n";
        targetCoordStr += $"{{\"latitude\": {this.Target.latitude}, \"longitude\": {this.Target.longitude}}}";

        string SearchAreaCoordStr = "\n[\n";
        int coordCount = this.SearchArea.Length;
        for (int i = 0; i < coordCount; i++) {
            SearchAreaCoordStr += $"{{\"latitude\": {this.SearchArea[i].latitude}, \"longitude\": {this.SearchArea[i].longitude}}}";
            if (i != coordCount - 1) {
                SearchAreaCoordStr += ",\n";
            } else {
                SearchAreaCoordStr += "\n]";
            }
        }
            // return data to post in json format
            return $"{{\n\"IsManual?\": {this.IsManual},\n \"TargetCoordinates\": {targetCoordStr},\n \"SearchArea\": {SearchAreaCoordStr}\n}}";
    }
}
}