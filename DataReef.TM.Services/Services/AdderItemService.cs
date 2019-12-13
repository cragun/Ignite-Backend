using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using DataReef.Core.Infrastructure.Repository;
using DataReef.Core.Logging;
using DataReef.TM.Contracts.Services;
using DataReef.TM.DataAccess.Database;
using DataReef.TM.Models.Solar;

namespace DataReef.TM.Services.Services
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(AddressFilterMode = AddressFilterMode.Any)]
    public class AdderItemService : DataService<AdderItem>, IAdderItemService
    {
        public AdderItemService(ILogger logger, Func<IUnitOfWork> unitOfWorkFactory) : base(logger, unitOfWorkFactory)
        {
        }

        /// <summary>
        /// Gets the kwh remaining after applying the reduction of the adder items
        /// </summary>
        public int ApplyAdderUsageReduction(int usageInKwh, IEnumerable<Guid> adderItemIds)
        {
            if (adderItemIds == null || !adderItemIds.Any())
                return usageInKwh;

            List<AdderItem> adderItems;
            using (var context = new DataContext())
            {
                adderItems = context.AdderItems
                    .Where(ai => adderItemIds.Contains(ai.Guid))
                    .ToList();

                if (!adderItems.Any()) return usageInKwh;
            }

            var addersUsageReduction = (int)adderItems.Sum(adderItem => adderItem.GetAdderUsageReduction(usageInKwh));

            return usageInKwh - addersUsageReduction;
        }

        /// <summary>
        /// Gets the cost after applying the adder items costs
        /// </summary>
        /// <param name="adderItemIds"></param>
        /// <returns></returns>
        public decimal GetAdderCosts(IEnumerable<Guid> adderItemIds)
        {
            if (adderItemIds == null || !adderItemIds.Any())
                return 0;

            List<AdderItem> adderItems;
            using (var context = new DataContext())
            {
                adderItems = context.AdderItems
                    .Include(ai => ai.RoofPlaneDetails.Select(rpd => rpd.RoofPlane.SolarPanel))
                    .Include(ai => ai.SolarSystem)
                    .Where(ai => adderItemIds.Contains(ai.Guid))
                    .ToList();

                if (!adderItems.Any()) return 0;
            }

            var adderCosts = adderItems.Sum(adderItem => adderItem.GetAdderCosts());

            return adderCosts;
        }
    }
}
