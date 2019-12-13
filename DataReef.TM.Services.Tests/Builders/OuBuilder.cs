using System;
using DataReef.TM.Models;

namespace DataReef.TM.Services.Tests.Builders
{
    public class OuBuilder : TestBuilder<OU>
    {
        public OuBuilder WithGuid()
        {
            Object.Guid = Guid.NewGuid();
            return this;
        }

        public OuBuilder WithParent(OU parentOU)
        {
            Object.Parent = parentOU;
            Object.ParentID = parentOU.Guid;
            return this;
        }
    }
}