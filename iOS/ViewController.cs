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
        OBADataProvider ob;
        Task dataPump;
        CoreLocation.CLLocationManager locationManager;

        private ClientLocation lastLocation = new ClientLocation();
        private RouteTableViewSource routeTableSource;

        public ViewController(IntPtr handle) : base(handle)
        {
            OBADataProvider.Options opt = new OBADataProvider.Options { URL = new Uri("http://api.pugetsound.onebusaway.org"), APIKey = "efdf5397-9338-447c-832a-ac93ad4c9151" };
            ob = new OBADataProvider(opt);
            CancellationTokenSource cts = new CancellationTokenSource();

            this.locationManager = CreateLocationManager();
            this.locationManager.LocationsUpdated += LocationManager_InitialLocationUpdated;
            this.locationManager.RequestLocation();

            dataPump = new Task(async () => await ob.Pump(cts.Token, this.GetClientLocation, this.OnTrip));

            this.routeTableSource = new RouteTableViewSource(ob, this);
            this.routeTableSource.OnRoutesChanged += RouteTableSource_OnRoutesChanged;

        }

        public override void LoadView()
        {
            base.LoadView();
            this.Results.RegisterClassForCellReuse(typeof(RouteCell), Constants.RouteCell);
        }

        private CLLocationManager CreateLocationManager()
        {

            // configure location watcher
            CLLocationManager result = new CLLocationManager();
            if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
            {
                result.RequestAlwaysAuthorization();
            }
            result.AllowsBackgroundLocationUpdates = true;
            result.DistanceFilter = CLLocationDistance.FilterNone;
            result.DesiredAccuracy = CLLocation.AccurracyBestForNavigation;
            result.PausesLocationUpdatesAutomatically = false;

            return result;
        }

        private void OnTrip(DataModels.Trip t) 
        {
            
        }

        public ClientLocation GetClientLocation()
        {
            return lastLocation;
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

            this.locationManager.StartMonitoringSignificantLocationChanges();
            this.locationManager.StartUpdatingLocation();

            this.Results.WeakDelegate = this;
            this.Results.WeakDataSource = this;
            this.Results.Source = this.routeTableSource;
            this.Results.ReloadData();
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.        
        }

        void LocationManager_InitialLocationUpdated(object sender, CLLocationsUpdatedEventArgs e)
        {
            LocationManager_LocationsUpdated(sender, e);

            // kick the pump
            this.dataPump.Start();
        
            // rewire the notifications
            this.locationManager.LocationsUpdated -= LocationManager_InitialLocationUpdated;
            this.locationManager.LocationsUpdated += LocationManager_LocationsUpdated;
        }

        void LocationManager_LocationsUpdated(object sender, CLLocationsUpdatedEventArgs e)
        {
            var currentLocation = e.Locations[0];
            this.lastLocation = new ClientLocation { Latitude = currentLocation.Coordinate.Latitude, Longitude = currentLocation.Coordinate.Longitude, Altitude = currentLocation.Altitude };
        }

        void RouteTableSource_OnRoutesChanged(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("RouteTableSource_OnRoutesChanged");
            this.InvokeOnMainThread(() => { this.Results.AllowsSelection = true; this.Results.ReloadData(); });
        }
    }
}
