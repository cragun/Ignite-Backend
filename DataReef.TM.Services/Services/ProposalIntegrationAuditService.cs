using DataReef.TM.Contracts.Services;
using DataReef.TM.Models.Solar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Text;
using System.Threading.Tasks;
using DataReef.Core.Infrastructure.Repository;
using DataReef.Core.Logging;

namespace DataReef.TM.Services.Services
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(AddressFilterMode = AddressFilterMode.Any)]

    public class ProposalIntegrationAuditService : DataService<ProposalIntegrationAudit>, IProposalIntegrationAuditService
    {
        public ProposalIntegrationAuditService(ILogger logger, Func<IUnitOfWork> unitOfWorkFactory) :
            base(logger, unitOfWorkFactory)
        {
        }
    }
}
