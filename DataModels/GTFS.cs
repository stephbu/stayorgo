using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace StayOrGo.DataModels
{
    
    [DataContract]
    public class Response 
    {
        [DataMember(Name="version")]
		public string Version;

		[DataMember(Name = "code")]
		public uint Code;

		[DataMember(Name = "currentTime")]
		public long CurrentTime;

		[DataMember(Name = "text")]
		public string Text;

    }

    [DataContract]
    public class RoutesResponse : Response
    {
        [DataMember(Name = "data")]
        public RoutesPayload Data;
    }

    [DataContract]
    public class Payload
    {
        [DataMember(Name="limitExceeded")]
        public bool LimitExceeded;

        [DataMember(Name = "outOfRange")]
        public bool OutOfRange;

		[DataMember(Name = "references")]
		public References References;
    }

	[DataContract]
	public class RoutesPayload
    {
        
		[DataMember(Name = "list")]
		public List<Route> List;
	}

    [DataContract]
    public class References
    {
        [DataMember(Name="agencies")]
		public List<Agency> Agencies;

		[DataMember(Name = "vehicles")]
		public List<Vehicle> Vehicles;

		[DataMember(Name = "routes")]
		public List<Route> Routes;
    }

	[DataContract]
	public class TransportObjects {};

    [DataContract]
    public class Agency : TransportObjects
    {
        [DataMember(Name="id")]
        public string ID;

		[DataMember(Name = "name")]
		public string Name;
	}

	[DataContract(Name="vehicle")]
	public class Vehicle : TransportObjects
    {
		[DataMember(Name = "id")]
		public string ID;
        public string LicencePlate;
        public string Label;
    }

    public class Trip : TransportObjects
    {
        public string TripID;
        public string RouteID;
        public string ServiceID;
        public string Headsign;
        public string ShortName;
        public string DirectionID;
        public string BlockID;
        public string ShapeID;
        public bool WheelchairAccessible;
        public bool BikesAllowed;
    }

	[DataContract(Name = "route")]
	public class Route : TransportObjects
    {
		[DataMember(Name = "id")]
		public string ID;

		[DataMember(Name = "agencyId")]
		public string AgencyID;

		[DataMember(Name = "shortName")]
		public string ShortName;

		[DataMember(Name = "longName")]
		public string LongName;

		[DataMember(Name = "description")]
		public string Description;

		[DataMember(Name = "type")]
		public uint Type;

		[DataMember(Name = "url")]
		public string URL;

        // TODO : marshal these correctly
        // public UInt32 Color;
        // public UInt32 TextColor;
    }

    public class Stop
    {
        public string StopID;
        public string Code;
        public string Name;
        public string Description;
        public float Latitude;
        public float Longitude;
        public string ZoneID;
        public string URL;
        public int LocationType;
        public string ParentStation;
        public string Timezone;
        public bool WheelchairBoarding;
    }
}
