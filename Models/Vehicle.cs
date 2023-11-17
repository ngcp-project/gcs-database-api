namespace Database.Models
{
    public class Vehicle
    {
        private float speed, pitch, yaw, roll, altitude, batteryLife;
        private String lastUpdated;
        private Coordinate currentPosition;
        private Status_Enum vehicleStatus;


        public Vehicle() {

        }
    }
}