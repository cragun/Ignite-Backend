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
    [RoutePrefix("api/v1/sprucegendocsrequests")]
    public class SpruceGenDocsRequestController : EntityCrudController<GenDocsRequest>
    {
        public SpruceGenDocsRequestController(ISpruceGenDocsRequestService service, ILogger logger)
            : base(service, logger)
        {
        }
    }
}