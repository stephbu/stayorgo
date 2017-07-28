using System;
namespace StayOrGo.DataModels
{
    public class Response 
    {
        public string Version;
        public string Code;
        public long CurrentTime;
        public string Text;
        public Payload Data;
    }

    public class Payload
    {
        public References References;
        public TransportObjects[] List;
    }

    public class References
    {
    }

    public abstract class TransportObjects {};

    public class Vehicle : TransportObjects
    {
        public string VehicleID;
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

    public class Route : TransportObjects
    {
        public string RouteID;
        public string AgencyID;
        public string ShortName;
        public string LongName;
        public string Description;
        public uint Type;
        public string URL;
        public UInt32 Color;
        public UInt32 TextColor;
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
