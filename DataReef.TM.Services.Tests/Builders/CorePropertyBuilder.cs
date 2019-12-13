using System;
using System.Collections.Generic;
using System.Linq;
using DataReef.TM.Models;
using DataReef.TM.Models.Geo;

namespace DataReef.TM.Services.Tests.Builders
{
    public class CorePropertyBuilder : TestBuilder<Property>
    {
        public CorePropertyBuilder WithGuid()
        {
            Object.Guid = Guid.NewGuid();
            return this;
        }

        public CorePropertyBuilder WithTerritory(Territory territory)
        {
            Object.Territory = territory;
            Object.TerritoryID = territory.Guid;
            return this;
        }

        public CorePropertyBuilder AsDeleted()
        {
            Object.IsDeleted = true;
            return this;
        }

        public CorePropertyBuilder WithDateCreated(DateTime dateCreated)
        {
            Object.DateCreated = dateCreated;
            return this;
        }

        public CorePropertyBuilder WithName(string name)
        {
            Object.Name = name;
            return this;
        }

        public CorePropertyBuilder WithDateLastModified(DateTime dateModified)
        {
            Object.DateLastModified = dateModified;
            return this;
        }

        public CorePropertyBuilder WithExternalID(string externalID)
        {
            Object.ExternalID = externalID;
            return this;
        }

        public CorePropertyBuilder WithOccupants(List<Occupant> occupants)
        {
            if (occupants == null)
                return this;

            Object.Occupants = occupants.ToList();
            occupants.ForEach(o => o.PropertyID = Object.Guid);

            return this;
        }

        public CorePropertyBuilder WithPropertyBag(List<Field> fields)
        {
            if (fields == null)
                return this;

            Object.PropertyBag = fields;
            fields.ForEach(f => f.PropertyId = Object.Guid);

            return this;
        }

        public CorePropertyBuilder WithAttributes(List<PropertyAttribute> attributes)
        {
            if (attributes == null)
                return this;

            Object.Attributes = attributes;
            attributes.ForEach(a => a.PropertyID = Object.Guid);

            return this;
        }
    }
}
