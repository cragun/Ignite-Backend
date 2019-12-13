using DataReef.Core.Logging;
using DataReef.TM.Contracts.Services;
using DataReef.TM.Models.Spruce;
using System.Web.Http;

namespace DataReef.TM.Api.Controllers
{
    [RoutePrefix("api/v1/quoterequests")]
    public class SpruceQuoteRequestsController : EntityCrudController<QuoteRequest>
    {
        public SpruceQuoteRequestsController(ISpruceQuoteRequestService service, ILogger logger)
            : base(service, logger)
        {
        }
    }
}