using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.Integrations.Core.Models
{
    public class CreateProposalRequest
    {
        public string ProposalBytes     { get; set; }

        public string PricingQuoteId    { get; set; }
    }
}
