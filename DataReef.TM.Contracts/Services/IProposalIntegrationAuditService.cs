using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using DataReef.TM.Models.Solar;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Contracts.Services
{
    [ServiceContract]
    [ServiceKnownType("GetKnownTypes", typeof(KnownTypesProvider))]
    public interface IProposalIntegrationAuditService : IDataService<ProposalIntegrationAudit>
    {
    }
}
