using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using DataReef.TM.Contracts.Services;
using DataReef.TM.Models.DTOs.FinanceAdapters;

namespace DataReef.TM.Api.Controllers
{
    [RoutePrefix("api/v1/servicefinance")]
    public class ServiceFinanceController : ApiController
    {
        private readonly IServiceFinanceAdapter _serviceFinanceAdapter;

        public ServiceFinanceController(IServiceFinanceAdapter serviceFinanceAdapter)
        {
            _serviceFinanceAdapter = serviceFinanceAdapter;
        }

        [Route("submitapplication")]
        [HttpPost]
        public async Task<HttpResponseMessage> SubmitApplication([FromBody]SubmitApplicationRequest request)
        {
            if (request.DownPayment > request.TotalCost)
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Down payment greater than Total Cost");

            if (!ModelState.IsValid)
                return Request.CreateResponse(HttpStatusCode.BadRequest, ModelState.Select(m => m.Value.Errors.Select(e => e.ErrorMessage)));

            var response = _serviceFinanceAdapter.SubmitApplication(request);

            return Request.CreateResponse(HttpStatusCode.OK, response);
        }
    }
}
