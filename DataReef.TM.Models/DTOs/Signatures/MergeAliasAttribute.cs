using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataReef.TM.Api.Classes.Hancock
{
    public class MergeAliasAttribute:System.Attribute
    {

        public MergeAliasAttribute(string alias)
        {
            this.Alias = alias;
        }

        public string Alias { get; set; }
    }
}