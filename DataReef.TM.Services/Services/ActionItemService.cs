using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using DataReef.Core.Infrastructure.Repository;
using DataReef.Core.Logging;
using DataReef.TM.Contracts.Services;
using DataReef.TM.DataAccess.Database;
using DataReef.TM.Models;
using DataReef.TM.Models.DTOs.EPC;

namespace DataReef.TM.Services.Services
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(AddressFilterMode = AddressFilterMode.Any)]
    public class ActionItemService : DataService<PropertyActionItem>, IActionItemService
    {
        private readonly Func<IOUService> _ouServiceFactory;
        private readonly Func<IPropertyService> _propertyServiceFactory;

        public ActionItemService(
            ILogger logger,
            Func<IOUService> ouServiceFactory,
            Func<IPropertyService> propertyServiceFactory,
            Func<IUnitOfWork> unitOfWorkFactory) : base(logger, unitOfWorkFactory)
        {
            _ouServiceFactory = ouServiceFactory;
            _propertyServiceFactory = propertyServiceFactory;
        }

        public void UploadActionItems(Guid ouid, List<ActionItemInput> actionItems)
        {
            if (ouid == Guid.Empty)
                throw new ArgumentException(nameof(ouid));
            if (actionItems == null)
                throw new ArgumentException(nameof(actionItems));

            //  get ou hierarchy
            var guidList = new List<Guid>() { ouid };
            var ouGuids = _ouServiceFactory().GetHierarchicalOrganizationGuids(guidList);

            //  get properties including territory
            var inputProperties = actionItems.Select(i => i.PropertyID).Distinct().ToList();
            var properties = _propertyServiceFactory().GetMany(inputProperties, "Territory").ToList();
            if (properties.Any(p => p.Territory == null))
                throw new ApplicationException("Invalid property territory");
            if (properties.Any(p => !ouGuids.Contains(p.Territory.OUID)))
                throw new ApplicationException("Invalid action items");

            using (var uow = UnitOfWorkFactory())
            {
                var existingActionItems = uow.Get<PropertyActionItem>().Where(i => inputProperties.Contains(i.PropertyID)).ToList();

                //  update existing action items
                foreach (var item in actionItems.Where(i => i.Guid.HasValue).ToList())
                {
                    var existingActionItem = existingActionItems.FirstOrDefault(i => i.Guid == item.Guid.Value);
                    if (existingActionItem == null) continue;

                    if (item.PersonID.HasValue) existingActionItem.PersonID = item.PersonID.Value;
                    if (!string.IsNullOrWhiteSpace(item.Description))
                        existingActionItem.Description = item.Description;
                    if (item.Status.HasValue) existingActionItem.Status = item.Status.Value;
                    uow.Update(existingActionItem);
                }

                //  add new action items
                foreach (var item in actionItems.Where(i => !i.Guid.HasValue).ToList())
                {
                    if (!item.PersonID.HasValue || string.IsNullOrWhiteSpace(item.Description) || !item.Status.HasValue)
                        continue;

                    var actionItem = new PropertyActionItem
                    {
                        PropertyID = item.PropertyID,
                        PersonID = item.PersonID.Value,
                        Description = item.Description,
                        Status = item.Status.Value
                    };
                    uow.Add(actionItem);
                }

                uow.SaveChanges();
            }
        }

        public override PropertyActionItem Insert(PropertyActionItem entity)
        {
            throw new ApplicationException("Inserts are not supported for this object");
        }

        public override PropertyActionItem Insert(PropertyActionItem entity, DataContext dataContext)
        {
            throw new ApplicationException("Inserts are not supported for this object");
        }

        public override ICollection<PropertyActionItem> InsertMany(ICollection<PropertyActionItem> entities)
        {
            throw new ApplicationException("Inserts are not supported for this object");
        }

        public override PropertyActionItem Update(PropertyActionItem entity)
        {
            throw new ApplicationException("Updates are not supported for this object");
        }
    }
}
