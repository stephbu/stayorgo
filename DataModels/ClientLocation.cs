using System.Diagnostics;

namespace DataModels
{
    public class ClientLocation
    {
        public double Latitude;
        public double Longitude;
        public double Altitude;

        public override string ToString()
        {
            return string.Format("[ClientLocation ({0},{1},{2})]", this.Latitude, this.Longitude, this.Altitude);
        }
    }
}
