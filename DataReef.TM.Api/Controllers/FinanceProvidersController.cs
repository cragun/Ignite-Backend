using DataReef.TM.Models.Finance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using DataReef.Core.Logging;
using DataReef.TM.Contracts.Services;
using System.Net.Http;
using System.Net;
using DataReef.TM.Api.Classes.Requests;
using System.Threading.Tasks;

namespace DataReef.TM.Api.Controllers
{
    [RoutePrefix("api/v1/financeproviders")]
    public class FinanceProvidersController : EntityCrudController<FinanceProvider>
    {
        public FinanceProvidersController(IDataService<FinanceProvider> dataService, ILogger logger) : base(dataService, logger)
        {
        }

        #region Forbidden methods



        public override async Task<FinanceProvider> Post(FinanceProvider item)
        {
            throw new HttpResponseException(HttpStatusCode.Forbidden);
        }

        public override async Task<ICollection<FinanceProvider>> PostMany(ICollection<FinanceProvider> items)
        {
            throw new HttpResponseException(HttpStatusCode.Forbidden);
        }

        public override async Task<FinanceProvider> Put(FinanceProvider item)
        {
            throw new HttpResponseException(HttpStatusCode.Forbidden);
        }

        public override async Task<HttpResponseMessage> Delete(FinanceProvider item)
        {
            throw new HttpResponseException(HttpStatusCode.Forbidden);
        }

        public override async Task<HttpResponseMessage> DeleteByGuid(Guid guid)
        {
            throw new HttpResponseException(HttpStatusCode.Forbidden);
        }

        public override async Task<HttpResponseMessage> DeleteMany([FromBody] IDsListWrapperRequest req)
        {
            throw new HttpResponseException(HttpStatusCode.Forbidden);
        }

        #endregion
    }
}