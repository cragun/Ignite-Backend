using DataReef.TM.Models;
using DataReef.TM.Models.Solar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.DataAccess.Database.Configurations
{
    internal class PropertyFluentConfiguration : FluentEntityConfiguration<Property>
    {
        public PropertyFluentConfiguration()
        {
            HasOptional(p => p.Survey)
                .WithRequired(s => s.Property);
        }
    }
}
