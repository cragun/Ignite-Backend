using DataReef.TM.Contracts.Services;
using System;
using System.Threading.Tasks;
using System.Web.Http;

namespace DataReef.TM.Api.Controllers
{
    [RoutePrefix("api/v1/health")]
    [AllowAnonymous]
    public class HealthController : ApiController
    {
        private IAppSettingService _settingsService;

        public HealthController(IAppSettingService settingsService)
        {
            _settingsService = settingsService;
        }

        /// <summary>
        /// This method fails if CORE is unreachable or fails.
        /// We should avoid using this method in AWS. If something's wrong w/ CORE it will remove API instances from Load Balancer.
        /// </summary>
        /// <returns></returns>
        [Route("")]
        [HttpGet]
        public async Task<IHttpActionResult> GetHealth()
        {
            try
            {

                var isHealthy = _settingsService.IsHealthy();
                if (isHealthy)
                {
                    return Ok();
                }
                return InternalServerError();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// This method checks if this API instance is healthy.
        /// </summary>
        /// <returns></returns>
        [Route("check")]
        [HttpGet]
        public async Task<IHttpActionResult> CheckHealth()
        {
            return Ok();
        }
    }
}
