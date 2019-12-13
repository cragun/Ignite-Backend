using System;
using DataReef.TM.Models;

namespace DataReef.TM.Services.Tests.Builders
{
    public class PropertyAttributeBuilder : TestBuilder<PropertyAttribute>
    {
        public PropertyAttributeBuilder WithDateCreated(DateTime dateCreated)
        {
            Object.DateCreated = dateCreated;
            return this;
        }
    }
}