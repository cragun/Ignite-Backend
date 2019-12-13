using DataReef.TM.Models.Solar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models.DTOs.Proposals
{
    public class ProposalUnformattedDataViewConstructor
    {
        public Proposal Proposal { get; set; }
        public FinancePlan FinancePlan { get; set; }
        public ICollection<KeyValue> KeyValues { get; set; }
        public List<OUSetting> Settings { get; set; }
        public ICollection<ProposalIntegrationAudit> IntegrationAudits { get; set; }
    }
}
