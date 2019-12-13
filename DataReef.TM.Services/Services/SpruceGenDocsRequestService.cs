using DataReef.Core.Logging;
using DataReef.TM.Contracts.Services;
using DataReef.TM.Models.Spruce;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Text;
using System.Threading.Tasks;
using DataReef.Core.Infrastructure.Repository;

namespace DataReef.TM.Services.Services
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(AddressFilterMode = AddressFilterMode.Any)]
    public class SpruceGenDocsRequestService : DataService<GenDocsRequest>, ISpruceGenDocsRequestService
    {
        public SpruceGenDocsRequestService(ILogger logger, Func<IUnitOfWork> unitOfWorkFactory) : base(logger, unitOfWorkFactory) { }
    }
}
