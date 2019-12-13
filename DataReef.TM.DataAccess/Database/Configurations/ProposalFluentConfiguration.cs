using DataReef.TM.Models.Solar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.DataAccess.Database.Configurations
{
    internal class ProposalFluentConfiguration:FluentEntityConfiguration<Proposal>
    {
        public ProposalFluentConfiguration()
        {
            HasOptional(p => p.SolarSystem)
                .WithRequired(ss => ss.Proposal);

            HasOptional(p => p.Tariff)
                .WithRequired(t => t.Proposal);
        }
    }
}
