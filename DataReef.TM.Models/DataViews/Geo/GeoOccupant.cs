using DataReef.TM.Models.Geo;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataReef.TM.Models.DataViews.Geo
{
    public class GeoOccupant
    {
        public string FirstName { get; set; }

        public string MiddleInitial { get; set; }

        public string LastName { get; set; }

        public string LastNameSuffix { get; set; }

        public string PropertyID { get; set; }

        public string Guid { get; set; }

        public List<GeoField> PropertyBag { get; set; }

        public Occupant ToDatabaseEntity(System.Guid propertyId)
        {
            var newOccupantId = System.Guid.NewGuid();
            return new Occupant
            {
                Guid = newOccupantId,
                ExternalID = this.Guid,
                FirstName = this.FirstName,
                MiddleInitial = this.MiddleInitial,
                LastName = this.LastName,
                LastNameSuffix = this.LastNameSuffix,
                PropertyID = propertyId,
                PropertyBag = this.PropertyBag != null ? this.PropertyBag.Where(pb => !String.IsNullOrEmpty(pb.Value)).Select(pb => pb.ToDatabaseEntity(propertyId, newOccupantId)).ToList() : null
            };
        }
    }
}
