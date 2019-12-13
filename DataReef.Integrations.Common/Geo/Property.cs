using System;
using System.Collections.Generic;
using System.Linq;

namespace DataReef.Integrations.Common.Geo
{
    public class Property
    {
        private static readonly HashSet<string> HiddenFields = new HashSet<string> { "locationid", "residentialid", "personid", "fulfillmentflag", "propertyid", "lifestyle" };

        public Property()
        {
            this.Occupants = new List<Occupant>();
            // todo: get rid of the PropertyBag and Attributes and add the needed data as fields to this entity
            this.PropertyBag = new List<Field>();
            this.Attributes = new List<PropertyAttribute>();
        }

        public string Id { get; set; }

        [Obsolete("use ID with v2 ElasticSearch")]
        public string Guid { get; set; }

        public string RecordLocator { get; set; }

        public string Name { get; set; }

        public string HouseNumber { get; set; }

        public bool IsEven
        {
            get
            {
                int houseNumber = 0;
                if (int.TryParse(HouseNumber, out houseNumber))
                {
                    return houseNumber % 2 == 0;
                }
                return false;
            }
        }

        public string Address1 { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string ZipCode { get; set; }

        public string StreetName { get; set; }

        public string PlusFour { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public bool IsActive { get; set; }

        public int HouseHoldArrivalDate { get; set; }

        public string FileRecencyDate { get; set; }

        public List<PropertyAttribute> Attributes { get; set; }

        public List<Field> PropertyBag { get; set; }

        public List<Occupant> Occupants { get; set; }

        public string GetFormattedAddress()
        {
            return $"{Address1}, {City}, {State} {ZipCode}";
        }

        public Occupant GetMainOccupant()
        {
            int occupantsCount = Occupants?.Count ?? 0;
            if (occupantsCount > 0)
            {
                var mainOccupant = Occupants.First();
                if (occupantsCount == 1)
                {
                    if (String.IsNullOrEmpty(mainOccupant.FirstName + mainOccupant.LastName))
                    {
                        mainOccupant = null;
                    }
                }
                else
                {
                    mainOccupant = Occupants.FirstOrDefault(o => String.Format("{0} {1}", o.FirstName, o.LastName).ToUpperInvariant() == Name.ToUpperInvariant());
                    if (mainOccupant == null)
                    {
                        mainOccupant = Occupants.FirstOrDefault(o => !String.IsNullOrEmpty(o.FirstName) && Name.Contains(o.FirstName) && !String.IsNullOrEmpty(o.LastName) && Name.Contains(o.LastName));
                    }
                    if (mainOccupant == null)
                    {
                        mainOccupant = Occupants.OrderBy(o => o.LastName).ThenBy(o => o.FirstName).First();
                    }
                }
                return mainOccupant;
            }
            var propOccupantName = Name.FirstAndLastName();

            return new Occupant
            {
                FirstName = propOccupantName.Item1,
                LastName = propOccupantName.Item2
            };
        }
    }
}

