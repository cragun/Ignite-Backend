using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using DataReef.Core.Classes;
using DataReef.Core.Infrastructure.Repository;
using DataReef.Core.Logging;
using DataReef.TM.Contracts.Services;
using DataReef.TM.DataAccess.Database;
using DataReef.TM.Models;
using DataReef.TM.Models.DTOs.EPC;
using DataReef.TM.Models.Enums;
using Newtonsoft.Json;

namespace DataReef.TM.Services.Services
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(AddressFilterMode = AddressFilterMode.Any)]
    public class EpcStatusService : DataService<EpcStatus>, IEpcStatusService
    {
        private readonly Func<IOUSettingService> _ouSettingServiceFactory;

        public EpcStatusService(ILogger logger, Func<IOUSettingService> ouSettingServiceFactory, Func<IUnitOfWork> unitOfWorkFactory) : base(logger, unitOfWorkFactory)
        {
            _ouSettingServiceFactory = ouSettingServiceFactory;
        }

        public void UploadEpcStatuses(Guid ouid, List<EpcStatusInput> epcStatuses)
        {
            var ouSettings = _ouSettingServiceFactory().GetSettings(ouid, null);
            if (!ouSettings.ContainsKey(OUSetting.Epc_Statuses))
                return;

            var statusesSetting = ouSettings.FirstOrDefault(s => s.Key.Equals(OUSetting.Epc_Statuses, StringComparison.InvariantCultureIgnoreCase)).Value.Value;
            var statuses = JsonConvert.DeserializeObject<List<string>>(statusesSetting);
            epcStatuses = epcStatuses.Except(epcStatuses.Where(s => !statuses.Contains(s.StatusName)).ToList()).ToList();

            var properties = epcStatuses.Select(s => s.PropertyID).Distinct().ToList();
            using (var context = new DataContext())
            {
                var existingEpcStatuses = context.EpcStatuses.Where(s => properties.Contains(s.PropertyID)).ToList();

                foreach (var status in epcStatuses)
                {
                    var dbEpcStatus = existingEpcStatuses.FirstOrDefault(s =>
                        s.PropertyID == status.PropertyID && s.Status == status.StatusName &&
                        s.PersonID == status.SalesRepID);
                    if (dbEpcStatus != null)
                    {
                        dbEpcStatus.StatusProgress = status.StatusProgress;
                        dbEpcStatus.CompletionDate = status.StatusProgress == EpcStatusProgress.Complete ? DateTime.UtcNow : (DateTime?)null;
                    }
                    else
                    {
                        dbEpcStatus = new EpcStatus
                        {
                            PropertyID = status.PropertyID,
                            Status = status.StatusName,
                            PersonID = status.SalesRepID,
                            StatusProgress = status.StatusProgress,
                            CompletionDate = status.StatusProgress == EpcStatusProgress.Complete ? DateTime.UtcNow : (DateTime?)null
                        };
                        context.EpcStatuses.Add(dbEpcStatus);
                    }
                }

                context.SaveChanges();
            }
        }

        public override EpcStatus Insert(EpcStatus entity)
        {
            throw new ApplicationException("Inserts are not supported for this object");
        }

        public override EpcStatus Insert(EpcStatus entity, DataContext dataContext)
        {
            throw new ApplicationException("Inserts are not supported for this object");
        }

        public override ICollection<EpcStatus> InsertMany(ICollection<EpcStatus> entities)
        {
            throw new ApplicationException("Inserts are not supported for this object");
        }

        public override EpcStatus Update(EpcStatus entity)
        {
            throw new ApplicationException("Updates are not supported for this object");
        }

        public override SaveResult Delete(Guid uniqueId)
        {
            throw new ApplicationException("Deletes are not supported for this object");
        }

        public override ICollection<SaveResult> DeleteMany(Guid[] uniqueIds)
        {
            throw new ApplicationException("Deletes are not supported for this object");
        }
    }
}
