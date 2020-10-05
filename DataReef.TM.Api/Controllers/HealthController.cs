using DataReef.Core.Infrastructure.Authorization;
using DataReef.TM.Contracts.Services;
using DataReef.TM.DataAccess.Database;
using DataReef.TM.Models;
using Newtonsoft.Json.Linq;
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

        [AllowAnonymous]
        [Route("sunnova/calback")]
        [HttpPost]
        public async Task<IHttpActionResult> SunnovaCallBackApi([FromBody] dynamic body, string request = null)
        {
            string json = Convert.ToString(body);
            JObject jo = JObject.Parse(json);

            ApiLogEntry apilog = new ApiLogEntry();
            apilog.Id = Guid.NewGuid();
            apilog.User = SmartPrincipal.UserId.ToString();
            apilog.Machine = Environment.MachineName;
            apilog.RequestContentType = "SunnovaCallBackApi";
            apilog.RequestRouteTemplate = "";
            apilog.RequestRouteData = "";
            apilog.RequestIpAddress = "";
            apilog.RequestMethod = "";
            apilog.RequestHeaders = "";
            apilog.RequestTimestamp = DateTime.UtcNow;
            apilog.RequestUri = "SunnovaCallBackApi";
            apilog.ResponseContentBody = "";
            apilog.RequestContentBody = json;

            using (var dc = new DataContext())
            {
                dc.ApiLogEntries.Add(apilog);
                dc.SaveChanges();
            }

            return Ok(true);
        }

        [AllowAnonymous]
        [Route("sunnova/calback")]
        [HttpGet]
        public async Task<IHttpActionResult> SunnovaCallBackApiGet(string request = null)
        {
            ApiLogEntry apilog = new ApiLogEntry();
            apilog.Id = Guid.NewGuid();
            apilog.User = SmartPrincipal.UserId.ToString();
            apilog.Machine = Environment.MachineName;
            apilog.RequestContentType = "SunnovaCallBackApiGet";
            apilog.RequestRouteTemplate = "";
            apilog.RequestRouteData = "";
            apilog.RequestIpAddress = "";
            apilog.RequestMethod = "";
            apilog.RequestHeaders = "";
            apilog.RequestTimestamp = DateTime.UtcNow;
            apilog.RequestUri = "SunnovaCallBackApiGet";
            apilog.ResponseContentBody = "";
            apilog.RequestContentBody = request;

            using (var dc = new DataContext())
            {
                dc.ApiLogEntries.Add(apilog);
                dc.SaveChanges();
            }

            return Ok(true);
        }
    }
}
