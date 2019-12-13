using System;
using DataReef.Integrations.Common.Geo;

namespace DataReef.TM.Services.Tests.Builders
{
    public class GeoPropertyAttributeBuilder : TestBuilder<PropertyAttribute>
    {
        public GeoPropertyAttributeBuilder WithDateCreated(DateTime dateCreated)
        {
            Object.DateCreated = dateCreated;
            return this;
        }
    }
}