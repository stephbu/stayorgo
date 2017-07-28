using System;
using System.Threading;
using System.Threading.Tasks;

using UIKit;
using MapKit;
using CoreLocation;

using DataModels;

namespace StayOrGo.iOS
{
    public partial class ViewController : UIViewController
    {
        OneBusAway ob;
        Task<bool> dataPump;
        CoreLocation.CLLocationManager locationManager;

        public ViewController(IntPtr handle) : base(handle)
        {
            OneBusAway.Options opt = new OneBusAway.Options { URL = new Uri("http://api.pugetsound.onebusaway.org"), APIKey="efdf5397-9338-447c-832a-ac93ad4c9151" };
            ob = new OneBusAway(opt);
            CancellationTokenSource cts = new CancellationTokenSource();

			// configure location watcher
			this.locationManager = new CLLocationManager();
			this.locationManager.PausesLocationUpdatesAutomatically = false;
			this.locationManager.RequestWhenInUseAuthorization();

			dataPump = ob.Pump(cts.Token, this.GetClientLocation, this.OnTrip);
        }

        private void OnTrip(DataModels.Trip t) 
        {
            
        }

        public ClientLocation GetClientLocation()
        {
            var currentLocation = this.locationManager.Location;
            return new ClientLocation { Latitude = currentLocation.Coordinate.Latitude, Longitude = currentLocation.Coordinate.Longitude, Altitude = currentLocation.Altitude };
        }


        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

			// configure Map
			Map.ShowsUserLocation = true;
			Map.ShowsBuildings = true;
            Map.ZoomEnabled = true;
            MKCoordinateRegion region = MKCoordinateRegion.FromDistance(this.locationManager.Location.Coordinate, 1000,1000);
            Map.SetRegion(region, false);
		}

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.		
        }
    }
}
