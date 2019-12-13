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

namespace DataReef.TM.Api.Controllers
{
    [RoutePrefix("api/v1/financeproviders")]
    public class FinanceProvidersController : EntityCrudController<FinanceProvider>
    {
        public FinanceProvidersController(IDataService<FinanceProvider> dataService, ILogger logger) : base(dataService, logger)
        {
        }

        #region Forbidden methods

        public override FinanceProvider Post(FinanceProvider item)
        {
            throw new HttpResponseException(HttpStatusCode.Forbidden);
        }

        public override ICollection<FinanceProvider> PostMany(List<FinanceProvider> items)
        {
            throw new HttpResponseException(HttpStatusCode.Forbidden);
        }

        public override FinanceProvider Put(FinanceProvider item)
        {
            throw new HttpResponseException(HttpStatusCode.Forbidden);
        }

        public override HttpResponseMessage Delete(FinanceProvider item)
        {
            throw new HttpResponseException(HttpStatusCode.Forbidden);
        }

        public override HttpResponseMessage DeleteByGuid(Guid guid)
        {
            throw new HttpResponseException(HttpStatusCode.Forbidden);
        }

        public override HttpResponseMessage DeleteMany([FromBody] IDsListWrapperRequest req)
        {
            throw new HttpResponseException(HttpStatusCode.Forbidden);
        }

        #endregion
    }
}