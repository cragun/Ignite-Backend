using System;
using System.Linq;
using DataReef.Core.Logging;
using DataReef.TM.Contracts.Services;
using DataReef.TM.Models;
using DataReef.TM.DataAccess.Database;
using DataReef.Core.Classes;
using DataReef.Core.Infrastructure.Authorization;
using System.ServiceModel.Activation;
using System.ServiceModel;
using DataReef.Core.Services;
using System.Collections.Generic;
using System.Data.Entity;
using DataReef.Core.Infrastructure.Repository;

namespace DataReef.TM.Services
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(AddressFilterMode = AddressFilterMode.Any)]
    public class CurrentLocationService : DataService<CurrentLocation>, ICurrentLocationService
    {

        public CurrentLocationService(ILogger logger, Func<IUnitOfWork> unitOfWorkFactory)
            : base(logger, unitOfWorkFactory)
        {

        }


        public System.Collections.Generic.IEnumerable<CurrentLocation> GetCurrentLocationsForPersonAndDate(Guid personID, DateTime date)
        {

            List<CurrentLocation> ret = new List<CurrentLocation>();

            using (DataContext dc = new DataContext())
            {
                ret = dc.CurrentLocations.Where(cc => DbFunctions.TruncateTime(cc.DateCreated) == date && cc.PersonID == personID).ToList();

            }

            return ret;
        }

        public ICollection<CurrentLocation> GetLatestLocations(List<Guid> personIds)
        {
            using (var uow = UnitOfWorkFactory())
            {
                return uow
                        .Get<CurrentLocation>()
                        .Where(cl => personIds.Contains(cl.PersonID)
                                  && !cl.IsDeleted
                                  && (cl.Lat != 0 || cl.Lon != 0 || cl.Accuracy != 0))
                        .GroupBy(cl => cl.PersonID)
                        .Select(gcl => gcl.OrderByDescending(cl => cl.DateCreated).FirstOrDefault())
                        .AsNoTracking()
                        .ToList();
            }
        }
    }
}
