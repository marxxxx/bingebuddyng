using System;
using System.Collections.Generic;
using System.Text;

namespace BingeBuddyNg.Infrastructure.Generated
{
    public class GoogleGeoCodeResponse
    {
        public Plus_Code plus_code { get; set; }
        public Result[] results { get; set; }
        public string status { get; set; }
    }

    public class Plus_Code
    {
        public string compound_code { get; set; }
        public string global_code { get; set; }
    }

    public class Result
    {
        public Address_Components[] address_components { get; set; }
        public string formatted_address { get; set; }
        public Geometry geometry { get; set; }
        public string place_id { get; set; }
        public Plus_Code1 plus_code { get; set; }
        public string[] types { get; set; }
    }

    public class Geometry
    {
        public _Location location { get; set; }
        public string location_type { get; set; }
        public Viewport viewport { get; set; }
    }

    public class _Location
    {
        public float lat { get; set; }
        public float lng { get; set; }
    }

    public class Viewport
    {
        public Northeast northeast { get; set; }
        public Southwest southwest { get; set; }
    }

    public class Northeast
    {
        public float lat { get; set; }
        public float lng { get; set; }
    }

    public class Southwest
    {
        public float lat { get; set; }
        public float lng { get; set; }
    }

    public class Plus_Code1
    {
        public string compound_code { get; set; }
        public string global_code { get; set; }
    }

    public class Address_Components
    {
        public string long_name { get; set; }
        public string short_name { get; set; }
        public string[] types { get; set; }
    }

}
