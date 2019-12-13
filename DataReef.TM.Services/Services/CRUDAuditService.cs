using DataReef.Core.Enums;
using DataReef.Core.Infrastructure.Repository;
using DataReef.Core.Logging;
using DataReef.TM.Contracts.Services;
using DataReef.TM.Models;
using System;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;

namespace DataReef.TM.Services.Services
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(AddressFilterMode = AddressFilterMode.Any)]

    public class CRUDAuditService : DataService<CRUDAudit>, ICRUDAuditService
    {
        public CRUDAuditService(ILogger logger, Func<IUnitOfWork> unitOfWorkFactory) : base(logger, unitOfWorkFactory)
        {
        }

        public void DeleteEntity<T>(T entity, string action) where T : EntityBase
        {
            if (entity == null)
            {
                return;
            }
            var audit = new CRUDAudit
            {
                EntityID = entity.Guid,
                Action = CrudAction.Delete,
                EntityName = typeof(T).Name,
                OldValue = entity?.Serialize(),
                TagString = action,
            };

            var uow = UnitOfWorkFactory();
            uow.Add(audit);
            uow.SaveChanges(DataSaveOperationContext.Insert);
        }

        public void DeleteEntities<T>(T[] entities, string action) where T : EntityBase
        {
            var uow = UnitOfWorkFactory();

            var audits = entities?.Select(e => new CRUDAudit
            {
                EntityID = e.Guid,
                Name = e.Name,
                Action = CrudAction.Delete,
                EntityName = typeof(T).Name,
                OldValue = e?.Serialize(),
                TagString = action,
            });

            if (audits?.Any() == true)
            {
                uow.AddMany(audits);
                uow.SaveChanges(DataSaveOperationContext.Insert);
            }
        }

        public void UpdateEntity<T>(T original, T updated, string action) where T : EntityBase
        {
            if (original == null)
            {
                return;
            }
            var audit = new CRUDAudit
            {
                EntityID = original.Guid,
                Action = CrudAction.Update,
                EntityName = typeof(T).Name,
                OldValue = original?.Serialize(),
                NewValue = updated?.Serialize(),
                TagString = action,
            };

            var uow = UnitOfWorkFactory();
            uow.Add(audit);
            uow.SaveChanges(DataSaveOperationContext.Insert);
        }

        public void UpdateValue(Guid guid, string name, string entityName, string originalValue, string newValue, string action)
        {
            var audit = new CRUDAudit
            {
                EntityID = guid,
                Name = name,
                Action = CrudAction.Update,
                EntityName = entityName,
                OldValue = originalValue,
                NewValue = newValue,
                TagString = action,
            };
            var uow = UnitOfWorkFactory();
            uow.Add(audit);
            uow.SaveChanges(DataSaveOperationContext.Insert);
        }
    }
}
