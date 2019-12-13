using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.Integrations.Microsoft.PowerBI.Models
{
    public class PBI_ProposalSigned : PBI_Base
    {
        public Guid ProposalID { get; set; }
        public double UtilityInflationRate { get; set; }

    }
}
