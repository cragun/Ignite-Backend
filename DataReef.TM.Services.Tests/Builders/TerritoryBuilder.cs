using System;
using DataReef.TM.Models;

namespace DataReef.TM.Services.Tests.Builders
{
    public class TerritoryBuilder : TestBuilder<Territory>
    {
        public TerritoryBuilder WithGuid()
        {
            Object.Guid = Guid.NewGuid();
            return this;
        }

        public TerritoryBuilder WithOU(OU ou)
        {
            Object.OU = ou;
            Object.OUID = ou.Guid;
            return this;
        }
    }
}