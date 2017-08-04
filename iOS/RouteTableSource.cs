using System;
using UIKit;
using Foundation;
using System.Collections.Generic;

namespace StayOrGo.iOS
{
    public class RouteTableViewSource : UITableViewSource
    {
        public event EventHandler OnRoutesChanged;
        string CellIdentifier = "TableCell";

        private string[] sections = new string[]{""};
        private ViewController owner;

        List<DataModels.Route> routes = new List<DataModels.Route>();

        public RouteTableViewSource(IRouteProvider provider, ViewController owner)
        {
            this.OnRoutesChanged += delegate {};
            provider.OnLocationRoutesChanged += Provider_LocationRoutesChanged;
            this.owner = owner;
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return routes.Count;
        }

		[Foundation.Export("numberOfSectionsInTableView:")]
        public override nint NumberOfSections(UITableView tableView)
        {
            return sections.Length;
        }

        public override string[] SectionIndexTitles(UITableView tableView)
        {
            return sections;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            RouteCell cell = (RouteCell) tableView.DequeueReusableCell(Constants.RouteCell);
            DataModels.Route route = this.routes[indexPath.Row];
            if (cell == null)
			{ 
                cell = new RouteCell(UITableViewCellStyle.Subtitle, Constants.RouteCell);
            }

            cell.TextLabel.Text = route.ShortName;
            //cell.DetailTextLabel.Text = route.Description;

			return cell;
        }

        void Provider_LocationRoutesChanged(object sender, List<DataModels.Route> e)
        {
            System.Diagnostics.Debug.WriteLine("Provider_LocationRoutesChanged");
            this.routes = e;

            foreach(var route in e)
            {
                System.Diagnostics.Debug.WriteLine("({0}){1} - {2}",route.ID, route.ShortName,route.Description);
            }

            this.OnRoutesChanged(this, new EventArgs());
        }
    }

	public class RouteCell : UITableViewCell
	{
		public RouteCell(IntPtr ptr) : base(ptr)
		{
		}

        public RouteCell(UITableViewCellStyle style, string cellIdentifier) : base(style, cellIdentifier)
        {
        }


    }
}
