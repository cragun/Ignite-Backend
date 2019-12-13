using System;
using DataReef.TM.Models;
using DataReef.TM.Models.Enums;

namespace DataReef.TM.Services.Tests.Builders
{
    public class PropertyActionItemBuilder : TestBuilder<PropertyActionItem>
    {
        public PropertyActionItemBuilder WithGuid()
        {
            Object.Guid = Guid.NewGuid();
            return this;
        }

        public PropertyActionItemBuilder WithProperty(Property property)
        {
            Object.Property = property;
            Object.PropertyID = property.Guid;
            return this;
        }

        public PropertyActionItemBuilder WithPerson(Person person)
        {
            Object.Person = person;
            Object.PersonID = person.Guid;
            return this;
        }

        public PropertyActionItemBuilder WithDescription(string description)
        {
            Object.Description = description;
            return this;
        }

        public PropertyActionItemBuilder WithStatus(ActionItemStatus status)
        {
            Object.Status = status;
            return this;
        }
    }
}
