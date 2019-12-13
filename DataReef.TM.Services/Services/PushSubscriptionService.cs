using DataReef.TM.Contracts.Services;
using DataReef.TM.Models.PushNotifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Text;
using System.Threading.Tasks;
using DataReef.Core.Infrastructure.Repository;
using DataReef.Core.Logging;
using DataReef.Core.Classes;

namespace DataReef.TM.Services.Services
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(AddressFilterMode = AddressFilterMode.Any)]
    public class PushSubscriptionService : DataService<PushSubscription>, IPushSubscriptionService
    {
        public PushSubscriptionService(ILogger logger, Func<IUnitOfWork> unitOfWorkFactory) : base(logger, unitOfWorkFactory)
        {
        }

        public override PushSubscription Insert(PushSubscription entity)
        {
            using (var uow = UnitOfWorkFactory())
            {
                var item = uow
                            .Get<PushSubscription>()
                            .FirstOrDefault(ps => ps.Name == entity.Name
                                            && ps.ExternalID == entity.ExternalID
                                            && ps.DeviceId == entity.DeviceId);

                if (item != null)
                {
                    item.NotificationType = entity.NotificationType;

                    if (item.IsDeleted)
                    {
                        item.IsDeleted = false;
                        uow.SaveChanges();
                    }
                    item.SaveResult = SaveResult.SuccessfulInsert;
                    return item;
                }
            }

            return base.Insert(entity);
        }
    }
}
