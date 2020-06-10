using DataReef.TM.Contracts.Services;
using DataReef.TM.Models;
using DataReef.TM.Models.DTOs.Reports;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace DataReef.TM.Api.Controllers
{
    [RoutePrefix("api/v1/admin")]
    public class AdministrationController : ApiController
    {
        private readonly IAdministrationService _adminService;
        private readonly IDataService<User> _userService;

        public AdministrationController(IAdministrationService adminService, IDataService<User> userService)
        {
            _adminService = adminService;
            _userService = userService;
        }

        [HttpGet]
        [ResponseType(typeof(ICollection<User>))]
        [Route("finduser/{userNamePart}")]
        public async Task<IHttpActionResult> FindUser(string userNamePart, string include = "", string exclude = "", string fields = "", bool deletedItems = true)
        {
            var resultGuids = _adminService.FindUser(userNamePart);
            var results = _userService.GetMany(resultGuids, include, exclude, fields, deletedItems);
            return Ok<ICollection<User>>(results);
        }

        [HttpGet]
        [ResponseType(typeof(ICollection<AuthenticationSummary>))]
        [Route("reports/authentications")]
        public async Task<IHttpActionResult> GetAuthenticationsReport(DateTime? fromDate = null, DateTime? toDate = null)
        {
            if (!fromDate.HasValue) fromDate = DateTime.UtcNow.AddMonths(-1).Date;
            if (!toDate.HasValue) toDate = DateTime.UtcNow.Date;
            try
            {
                var results = _adminService.GetAuthenticationSummaries(fromDate, toDate);
                return Ok<ICollection<AuthenticationSummary>>(results);
            }
            catch
            {
                return Unauthorized();
            }
        }
    }
}
