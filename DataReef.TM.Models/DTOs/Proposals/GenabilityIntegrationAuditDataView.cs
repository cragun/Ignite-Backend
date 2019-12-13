using DataReef.TM.Models.Solar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models.DTOs.Proposals
{
    public class GenabilityIntegrationAuditDataView
    {
        public GenabilityIntegrationAuditDataView(ProposalIntegrationAudit audit)
        {
            Name = audit.Name;
            Url = audit.Url;
            RequestJSON = audit.RequestJSON;
            ResponseJSON = audit.ResponseJSON;
        }

        public string Name { get; set; }
        public string Url { get; set; }
        public string RequestJSON { get; set; }
        public string ResponseJSON { get; set; }
    }
}
