using DataReef.Core.Infrastructure.Authorization;
using DataReef.Core.Logging;
using DataReef.TM.Contracts.Services;
using DataReef.TM.Models;
using DataReef.TM.Models.DTOs.Properties;
using DataReef.TM.Models.DTOs.PropertyAttachments;
using DataReef.TM.Models.Solar;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using WebApi.OutputCache.V2;

namespace DataReef.TM.Api.Controllers
{
    /// <summary>
    /// Managers CRUD for Properties
    /// </summary>
    [RoutePrefix("api/v1/propertysurveys")]
    public class PropertYSurveysController : EntityCrudController<PropertySurvey>
    {
        private readonly IPropertySurveyService _propertySurveyService;

        public PropertYSurveysController(IPropertySurveyService propertySurveyService,
                                    ILogger logger)
            : base(propertySurveyService, logger)
        {
            this._propertySurveyService = propertySurveyService;
        }

        /// <summary>
        /// / Gets paged property surveys for the specified userID
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="pageIndex"></param>
        /// <param name="itemsPerPage"></param>
        /// <returns></returns>
        [Route("user/{userID}")]
        [ResponseType(typeof(ICollection<PropertySurveyDTO>))]
        [HttpGet]
        public async Task<IHttpActionResult> GetAllForUser(Guid userID, int pageIndex = 0, int itemsPerPage = 20)
        {
            var result = _propertySurveyService.GetPropertySurveysForUser(userID, pageIndex, itemsPerPage);

            return Ok(result);
        }


        /// <summary>
        /// / Gets paged property surveys for the currently logged in user
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="itemsPerPage"></param>
        /// <returns></returns>
        [Route("user")]
        [ResponseType(typeof(ICollection<PropertySurveyDTO>))]
        [HttpGet]
        public async Task<IHttpActionResult> GetAllForCurrentUser(int pageIndex = 0, int itemsPerPage = 20)
        {
            var result = _propertySurveyService.GetPropertySurveysForUser(SmartPrincipal.UserId, pageIndex, itemsPerPage);

            return Ok(result);
        }

        /// <summary>
        /// / Gets the property survey DTO for the specified guid
        /// </summary>
        /// <returns></returns>
        [Route("single/{propertySurveyID}")]
        [ResponseType(typeof(ICollection<PropertySurveyDTO>))]
        [HttpGet]
        public async Task<IHttpActionResult> GetSingle(Guid propertySurveyID)
        {
            var result = _propertySurveyService.GetPropertySurveyDTO(propertySurveyID);

            return Ok(result);
        }
    }
}
