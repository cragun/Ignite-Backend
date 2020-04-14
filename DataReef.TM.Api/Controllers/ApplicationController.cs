using DataReef.Core;
using DataReef.Core.Logging;
using DataReef.TM.Api.Classes.Requests;
using DataReef.TM.Contracts.Services;
using System;
using System.Web.Http;

namespace DataReef.TM.API.Controllers
{
    [Authorize]
    [RoutePrefix("api/v1/application")]
    public class ApplicationController : ApiController
    {
        private IAppSettingService _settingsService;
        private ILogger _logger;

        public ApplicationController(IAppSettingService settingsService, ILogger logger)
        {
            _settingsService = settingsService;
            _logger = logger;
        }

        /// <summary>
        /// Get the iPad version
        /// </summary>
        /// <returns></returns>
        [Route("versions/ipad")]
        [HttpGet]
        public string GetIpadApplicationBuild()
        {
            try
            {

                return _settingsService.GetValue(Constants.IPadVersionSettingName);
            }
            catch (Exception ex)
            {
                _logger.Error("GetIpadApplicationBuild", ex);
                return String.Empty;
            }
        }

        /// <summary>
        /// Set the iPad version 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPatch]
        [Route("versions/ipad/set/{value}")]
        public bool SetIpadApplicationBuild(string value)
        {
            try
            {
                _settingsService.SetValue(Constants.IPadVersionSettingName, value);
                _settingsService.SetValue(Constants.IPadMinimumVersionSettingName, value);
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error("SetIpadApplicationBuild", ex);
            }
            return false;
        }

        /// <summary>
        /// Set LoginDays 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPatch]
        [Route("loginDays/{value}")]
        public bool SetLoginDays(string value)
        {
            try
            {
                _settingsService.SetValue(Constants.LoginDays, value);

                return true;
            }
            catch (Exception ex)
            {
                _logger.Error("SetLoginDays", ex);
            }
            return false;
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("push/test")]
        public bool TestPushNotifications(TestAPNPushRequest request)
        {
            _settingsService.TestPushNotifications(request.DeviceToken, request.Payload);
            return true;
        }
    }
}
