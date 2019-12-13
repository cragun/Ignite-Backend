using DataReef.Core.Logging;
using DataReef.TM.Contracts.Services;
using DataReef.TM.DataAccess.Database;
using DataReef.TM.Models;
using EntityFramework.Extensions;
using System;
using System.Linq;
using DataReef.Core.Infrastructure.Repository;

namespace DataReef.TM.Services
{
    public class ZipAreaService : DataService<ZipArea>, IZipAreaService
    {
        private readonly ILogger _logger;

        public ZipAreaService(ILogger logger, Func<IUnitOfWork> unitOfWorkFactory) : base(logger, unitOfWorkFactory)
        {
            _logger = logger;
        }

        public override ZipArea Update(ZipArea entity)
        {
            using (var dataContext = new DataContext())
            using (var transaction = dataContext.Database.BeginTransaction())
            {
                try
                {
                    // remove AreaPurchases
                    dataContext
                            .AreaPurchases
                            .Where(a => a.AreaID == entity.Guid)
                            .Delete();

                    dataContext.SaveChanges();

                    var ret = base.Update(entity);
                    if (!ret.SaveResult.Success) throw new Exception(ret.SaveResult.Exception + " " + ret.SaveResult.ExceptionMessage);
                    UpdateNavigationProperties(entity, dataContext: dataContext);

                    transaction.Commit();
                    return ret;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }
    }
}
