using DataReef.Core.Infrastructure.Authorization;
using DataReef.Core.Logging;
using DataReef.Integrations.Google.Models;
using DataReef.TM.Contracts.Services;
using DataReef.TM.Models;
using DataReef.TM.Models.DataViews;
using DataReef.TM.Models.DTOs.Persons;
using DataReef.TM.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;

namespace DataReef.TM.Api.Controllers
{
    /// <summary>
    /// Managers CRUD for PersonKPI.
    /// </summary>
    [RoutePrefix("api/v1/personkpis")]
    public class PersonKPIsController : EntityCrudController<PersonKPI>
    {
        private readonly IPersonKPIService personKPIsService;
        private readonly IPersonSettingService _personSettingService;

        public PersonKPIsController(IPersonKPIService service,
            ILogger logger,
            IPersonSettingService personSettingService)
            : base(service, logger)
        {
            this.personKPIsService = service;
            this._personSettingService = personSettingService;
        }

        /// <summary>
        /// Gets all Tallies for current user starting on the specified date
        /// </summary>
        /// <param name="date">Date from which to show Tallies. If not specified, the current date will be used</param>
        /// <returns></returns>
        [HttpGet]
        [Route("mine/{date?}")]
        [ResponseType(typeof(ICollection<PersonKPI>))]
        public IHttpActionResult GetMyKPIsForToday(DateTime? date = null)
        {
            DateTime computedDate = AdjustDateBasedOnClientOffsetOrNow(date);

            var ret = personKPIsService.ListKPIsFromDate(computedDate);
            return Ok(ret);
        }

        /// <summary>
        /// Used for uploading in a specific location a screenshot of the tallies 
        /// </summary>
        /// <param name="request">Contains the image to be saved in base64 format</param>
        /// <param name="date">Date for which the tallies screenshot is uploaded. If not specified, the current date will be used</param>
        /// <returns></returns>
        [HttpPost]
        [Route("mine/screenshot/{date?}")]
        public IHttpActionResult SaveScreenshot([FromBody]PersonKPIScreenshotRequest request, DateTime? date = null)
        {
            DateTime computedDate = AdjustDateBasedOnClientOffsetOrNow(date);

            var ret = personKPIsService.SaveKPIScreenShot(request.ScreenshotData, computedDate);

            return Ok(ret);
        }

        [HttpPost]
        [Route("mine/reset")]
        [ResponseType(typeof(ICollection<PersonKPI>))]
        public IHttpActionResult ResetKPIs(DateTime? date = null)
        {
            var resetDate = date ?? DateTime.UtcNow;
            personKPIsService.ResetKPIs(resetDate);

            var ret = personKPIsService.ListKPIsFromDate(resetDate);
            return Ok(ret);
        }

        [Route("generategooglesheet")]
        [HttpPost]
        [InjectAuthPrincipal]
        [AllowAnonymous]
        public IHttpActionResult GenerateGoogleSheet(List<Guid> OUIDs)
        {
            return Ok(new { });
        }

        #region Helpers

        private DateTime AdjustDateBasedOnClientOffsetOrNow(DateTime? date)
        {
            var offset = -SmartPrincipal.DeviceDate.Offset;

            return date?.Add(offset) ?? DateTime.UtcNow;
        }
        #endregion
    }
}