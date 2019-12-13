using System;
using DataReef.TM.Models;

namespace DataReef.TM.Services.Tests.Builders
{
    public class PersonBuilder : TestBuilder<Person>
    {
        public PersonBuilder WithGuid()
        {
            Object.Guid = Guid.NewGuid();
            return this;
        }
    }
}