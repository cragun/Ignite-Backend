using System;

namespace DataReef.TM.Models.DTOs.Signatures
{
    public class PropertyDataView
    {
        public PropertyDataView(Property property)
        {
            if (property == null) return;

            PropertyID = property.Guid;
            HouseNumber = property.HouseNumber;
            Address1 = property.Address1;
            Address2 = property.Address2;
            City = property.City;
            State = property.State;
            ZipCode = property.ZipCode;
            PlusFour = property.PlusFour;
            StreetName = property.StreetName;
            Latitude = property.Latitude;
            Longitude = property.Longitude;
            Name = property.Name;
        }

        public Guid PropertyID { get; set; }

        public string Name { get; set; }

        public double? Latitude { get; set; }

        public double? Longitude { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string ZipCode { get; set; }

        public string PlusFour { get; set; }

        public string StreetName { get; set; }

        public string HouseNumber { get; set; }

        public string Address1 { get; set; }

        public string Address2 { get; set; }
    }
}
