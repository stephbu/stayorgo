using System;
using System.Collections.Generic;

namespace StayOrGo
{
    public interface IRouteProvider
    {
		event EventHandler<List<DataModels.Route>> OnLocationRoutesChanged;
    }
}
