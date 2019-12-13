using System.Collections.Generic;
using DataReef.Integrations.Common.Geo;

namespace DataReef.TM.Services.Tests.Builders
{
    public class GeoPropertyBuilder : TestBuilder<Property>
    {
        public GeoPropertyBuilder WithId(string id)
        {
            Object.Id = id;
            return this;
        }

        public GeoPropertyBuilder WithOccupants(List<Occupant> occupants)
        {
            if (occupants == null)
                return this;

            Object.Occupants = occupants;

            return this;
        }

        public GeoPropertyBuilder WithPropertyBag(List<Field> fields)
        {
            if (fields == null)
                return this;

            Object.PropertyBag = fields;

            return this;
        }

        public GeoPropertyBuilder WithAttributes(List<PropertyAttribute> attributes)
        {
            if (attributes == null)
                return this;

            Object.Attributes = attributes;

            return this;
        }
    }
}