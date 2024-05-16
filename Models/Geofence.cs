namespace Database.Models
{
    public class Geofence
    {
        private bool isKeepIn;

        public Geofence(bool isKeepIn)
        {
            this.isKeepIn = isKeepIn;
        }

        public void setKeepIn(bool isKeepIn)
        {
            this.isKeepIn = isKeepIn;
        }

        public bool getKeepIn()
        {
            return isKeepIn;
        }
    }
}