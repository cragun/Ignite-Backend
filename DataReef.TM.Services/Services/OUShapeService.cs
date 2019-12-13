
using DataReef.Core.Classes;
using DataReef.Core.Logging;
using DataReef.TM.Contracts.Services;
using DataReef.TM.DataAccess.Database;
using DataReef.TM.Models;
using DataReef.TM.Services.InternalServices.Geo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using DataReef.Core.Infrastructure.Repository;

namespace DataReef.TM.Services.Services
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(AddressFilterMode = AddressFilterMode.Any)]
    public class OUShapeService : DataService<OUShape>, IOUShapeService
    {
        ILogger _logger;
        IGeoProvider _geoProvider;

        public OUShapeService(ILogger logger, IGeoProvider geoProvider, Func<IUnitOfWork> unitOfWorkFactory)
            : base(logger, unitOfWorkFactory)
        {
            _logger = logger;
            _geoProvider = geoProvider;
        }

        public override SaveResult Delete(Guid uniqueId)
        {
            using (var dataContext = new DataContext())
            {
                // get the OUShape
                var ouShape = dataContext.OUShapes.FirstOrDefault(o => o.Guid == uniqueId);

                if (ouShape == null)
                {
                    return new SaveResult
                    {
                        Success = false,
                        Exception = "Not found!"
                    };
                }

                // get all children OU Ids
                var ouIds = dataContext
                                    .Database
                                    .SqlQuery<OU>("exec [proc_SelectOUHierarchy] {0}", ouShape.OUID)
                                    .Where(o => !o.IsDeleted)
                                    .Select(o => o.Guid)
                                    .ToList();

                // creating a new list that contain all the children ouIds
                var ouIdsExcludingCurrent = ouIds
                                    .Except(new List<Guid> { ouShape.OUID })
                                    .ToList();

                // verify if any of the subOUs contains the shapeId that's about to be deleted
                var cannotDeleteShape = dataContext
                                        .OUShapes
                                        .Any(o => ouIdsExcludingCurrent.Contains(o.OUID) && o.ShapeID == ouShape.ShapeID && !o.IsDeleted);

                // reuse this when sending the failure message
                Func<string, SaveResult> response = (msg) =>
                {
                    var result = new SaveResult();
                    result.Success = false;
                    result.Exception = msg;
                    return result;
                };

                if (cannotDeleteShape)
                {
                    return response("Shape used in children OUs.");
                }

                // get all the territoryIds
                var territoryIds = dataContext
                                    .Territories
                                    .Where(t => ouIds.Contains(t.OUID) && !t.IsDeleted)
                                    .Select(t => t.Guid)
                                    .ToList();

                // verify if any of the territory inside OU and children OUs contain the shape that's about to be deleted.
                cannotDeleteShape = dataContext
                                    .TerritoryShapes
                                    .Any(ts => territoryIds.Contains(ts.TerritoryID) && ts.ShapeID == ouShape.ShapeID && !ts.IsDeleted);

                if (cannotDeleteShape)
                {
                    return response("Shape used in children territories.");
                }

                var shapeIds = dataContext
                                .OUShapes
                                .Where(s => ouIdsExcludingCurrent.Contains(s.OUID) && !s.IsDeleted)
                                .Select(s => s.ShapeID)
                                .Distinct()
                                .ToList();

                var territoryShapeIds = dataContext
                                .TerritoryShapes
                                .Where(ts => territoryIds.Contains(ts.TerritoryID) && !ts.IsDeleted)
                                .Select(t => t.ShapeID)
                                .Distinct()
                                .ToList();

                // get all the shapes from OUs and Territories
                shapeIds.AddRange(territoryShapeIds);

                // call the Geo service that verifies if any of the shapes is in the Shape Children list.
                if (!_geoProvider.CanDeleteShape(ouShape.ShapeID, shapeIds))
                {
                    return response("Sub-OUs or territories contain sub-shapes of this shape.");
                }
            }

            return base.Delete(uniqueId);
        }
    }
}
