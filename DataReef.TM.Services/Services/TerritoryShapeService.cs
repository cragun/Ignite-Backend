using DataReef.Core.Logging;
using DataReef.TM.Contracts.Services;
using DataReef.TM.Models;
using System;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using DataReef.Core.Classes;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Data.Entity.Spatial;
using DataReef.Core.Enums;
using DataReef.Core.Infrastructure.Repository;
using DataReef.TM.Services.Extensions;
using DataReef.TM.Services.Helpers;

namespace DataReef.TM.Services.Services
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(AddressFilterMode = AddressFilterMode.Any)]

    public class TerritoryShapeService : DataService<TerritoryShape>, ITerritoryShapeService
    {
        public TerritoryShapeService(ILogger logger, Func<IUnitOfWork> unitOfWorkFactory) :
            base(logger, unitOfWorkFactory)
        {
        }

        public override SaveResult Delete(Guid uniqueId)
        {
            using (var uow = UnitOfWorkFactory())
            {
                var entity = uow.Get<TerritoryShape>().FirstOrDefault(ts => ts.Guid == uniqueId);
                if (entity == null)
                    return new SaveResult
                    {
                        Action = DataAction.None,
                        EntityUniqueId = uniqueId,
                        Success = false
                    };

                if (string.IsNullOrWhiteSpace(entity.WellKnownText))
                    return base.Delete(uniqueId);

                var fixedWkt = GeographyHelper.FixWkt(entity.WellKnownText);

                //get the DbGeography representation of WKT
                var geo = DbGeography.FromText(fixedWkt);

                // get all the properties that belong to this territory
                // and their location is contained in the Shape's WKT
                var propertiesToDelete = uow.Get<Property>()
                    .Include(p => p.Inquiries)
                    .Where(p => p.IsDeleted == false
                                && p.Latitude.HasValue
                                && p.Longitude.HasValue
                                && p.TerritoryID == entity.TerritoryID
                                && SqlSpatialFunctions.Filter(geo, SqlSpatialFunctions.PointGeography(p.Latitude, p.Longitude, DbGeography.DefaultCoordinateSystemId)) == true)
                    .ToList();

                // soft delete properties
                foreach (var property in propertiesToDelete)
                {
                    uow.Delete(property);

                    // if the property has inquiries, soft delete them
                    if (property.Inquiries == null) continue;

                    uow.DeleteMany(property.Inquiries);
                }
            }

            return base.Delete(uniqueId);
        }
    }
}
