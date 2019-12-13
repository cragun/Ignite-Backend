using DataReef.TM.Models.Solar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.DataAccess.Database.Configurations
{
    internal class SolarSystemFluentConfiguration : FluentEntityConfiguration<SolarSystem>
    {
        public SolarSystemFluentConfiguration()
        {
            HasOptional(p => p.SystemProduction)
                .WithRequired(ss => ss.SolarSystem);

        }
    }
}
