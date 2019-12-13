using DataReef.TM.Contracts.Services;
using System.Data.Entity;
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
using DataReef.Integrations.Microsoft;
using DataReef.TM.DataAccess.Database;

namespace DataReef.TM.Services.Services
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(AddressFilterMode = AddressFilterMode.Any)]
    public class PropertyPowerConsumptionService : DataService<PropertyPowerConsumption>, IPropertyPowerConsumptionService
    {
        private readonly Lazy<IPowerBIBridge> _powerBIBridge;
        public PropertyPowerConsumptionService(
            ILogger logger,
            Lazy<IPowerBIBridge> powerBIBridge,
            Func<IUnitOfWork> unitOfWorkFactory)
            : base(logger, unitOfWorkFactory)
        {
            _powerBIBridge = powerBIBridge;
        }

        public override ICollection<PropertyPowerConsumption> InsertMany(ICollection<PropertyPowerConsumption> entities)
        {
            var result = base.InsertMany(entities);

            var propertyIds = result.GroupBy(pc => pc.PropertyID);
            var propIDs = propertyIds.Select(p => p.Key).ToList();
            using (var dataContext = new DataContext())
            {
                var properties = dataContext
                                    .Properties
                                    .Include(p => p.Territory)
                                    .Where(p => propIDs.Contains(p.Guid))
                                    .ToList();

                foreach (var propGroup in propertyIds)
                {
                    var property = properties.FirstOrDefault(p => p.Guid == propGroup.Key);
                    property.PowerConsumptions = propGroup.ToList();
                    var pbi = property.ToPBI(property.Territory.OUID);
                    _powerBIBridge.Value.PushDataAsync(pbi);
                }
            }
            return result;
        }
    }
}
