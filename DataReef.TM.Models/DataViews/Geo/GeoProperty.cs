using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models.DataViews.Geo
{
    public class GeoProperty
    {

        public Guid Guid { get; set; }

        public string RecordLocator { get; set; }

        public string Name { get; set; }

        public string HouseNumber { get; set; }

        public bool IsEven { get; set; }

        public string Address1 { get; set; }

        public string Address2 { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string ZipCode { get; set; }

        public string PlusFour { get; set; }

        public string DeliveryPoint { get; set; }

        public string StreetName { get; set; }

        public float Latitude { get; set; }

        public float Longitude { get; set; }

        public List<GeoField> PropertyBag { get; set; }

        public List<PropertyAttribute> Attributes { get; set; }

        public List<GeoOccupant> Occupants { get; set; }

    }
}
