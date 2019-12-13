using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataReef.Geography
{
    public class CoordinateGeoRequest : GeoRequest
    {
        public double Lat1 { get; set; }
        public double Lon1 { get; set; }
        public double Lat2 { get; set; }
        public double Lon2 { get; set; }

        public string ToWellKnownText()
        {
            return string.Format("POLYGON (({0} {1},{0} {3},{2} {3},{2} {1},{0} {1}))", this.Lon1.ToString(), this.Lat1.ToString(), this.Lon2.ToString(), this.Lat2.ToString());
        }

    }
}
