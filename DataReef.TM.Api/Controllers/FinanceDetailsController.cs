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
    [RoutePrefix("api/v1/financedetails")]
    public class FinanceDetailsController : EntityCrudController<FinanceDetail>
    {
        public FinanceDetailsController(IDataService<FinanceDetail> dataService, ILogger logger) : base(dataService, logger)
        {
        }

        #region Forbidden methods

        public override FinanceDetail Post(FinanceDetail item)
        {
            throw new HttpResponseException(HttpStatusCode.Forbidden);
        }

        public override ICollection<FinanceDetail> PostMany(List<FinanceDetail> items)
        {
            throw new HttpResponseException(HttpStatusCode.Forbidden);
        }

        public override FinanceDetail Put(FinanceDetail item)
        {
            throw new HttpResponseException(HttpStatusCode.Forbidden);
        }

        public override HttpResponseMessage Delete(FinanceDetail item)
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