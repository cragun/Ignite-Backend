using DataReef.Core.Logging;
using DataReef.TM.Contracts.Services;
using DataReef.TM.Models;
using System.Web.Http;

namespace DataReef.TM.Api.Controllers
{
    [RoutePrefix("api/v1/zipareas")]
    public class ZipAreaController : EntityCrudController<ZipArea>
    {
        public ZipAreaController(IZipAreaService zipAreaService, ILogger logger): base(zipAreaService, logger)
        {
        }
    }
}
