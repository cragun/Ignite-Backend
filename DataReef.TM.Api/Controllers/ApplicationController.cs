using DataReef.Core;
using DataReef.Core.Logging;
using DataReef.TM.Api.Classes.Requests;
using DataReef.TM.Contracts.Services;
using DataReef.TM.Models.DTOs.Persons;
using System;
using System.Threading.Tasks;
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
        public async Task<string> GetIpadApplicationBuild()
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
        public async Task<bool> SetIpadApplicationBuild(string value)
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
        /// Get the login Days
        /// </summary>
        /// <returns></returns>
        [Route("loginDays")]
        [HttpGet]
        public async Task<string> GetloginDays()
        {
            try
            {

                return _settingsService.GetValue(Constants.LoginDays);
            }
            catch (Exception ex)
            {
                _logger.Error("GetloginDays", ex);
                return String.Empty;
            }
        }

        /// <summary>
        /// Set LoginDays 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPatch]
        [Route("loginDays/{value}")]
        public async Task<bool> SetLoginDays(string value)
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

        /// <summary>
        /// Get the IOSVersion
        /// </summary>
        /// <returns></returns>
        [Route("IOSVersion")]
        [HttpGet]
        public async Task<string> GetIOSVersion()
        {
            try
            {

                return _settingsService.GetValue(Constants.IOSVersion);
            }
            catch (Exception ex)
            {
                _logger.Error("GetIOSVersion", ex);
                return String.Empty;
            }
        }

        //public class iosversionmodel
        //{
        //    public string VersionValue { get; set; }
        //    public bool IsPopupEnabled { get; set; }
        //}
        /// <summary>
        /// Set IOSVersion 
        /// </summary>
        /// <returns></returns>
        [HttpPatch]
        [Route("IOSVersion")]
        public async Task<bool> SetIOSVersion(IOSVersionDTO ios)
        {
            try
            {
                _settingsService.SetValue(Constants.IOSVersion, Newtonsoft.Json.JsonConvert.SerializeObject(ios));

                return true;
            }
            catch (Exception ex)
            {
                _logger.Error("SetIOSVersion", ex);
            }
            return false;
        }


        [HttpPost]
        [AllowAnonymous]
        [Route("push/test")]
        public async Task<bool> TestPushNotifications(TestAPNPushRequest request)
        {
            _settingsService.TestPushNotifications(request.DeviceToken, request.Payload);
            return true;
        }
    }
}
