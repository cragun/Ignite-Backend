using DataReef.Core.Logging;
using DataReef.TM.Contracts.Services;
using DataReef.TM.Models.Spruce;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace DataReef.TM.Api.Controllers
{
    [RoutePrefix("api/v1/quoteresponses")]
    public class SpruceQuoteResponseController : EntityCrudController<QuoteResponse>
    {
        public SpruceQuoteResponseController(ISpruceQuoteResponseService service, ILogger logger)
            : base(service, logger)
        {
        }
    }
}