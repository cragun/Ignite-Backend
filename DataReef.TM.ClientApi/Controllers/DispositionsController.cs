using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using DataReef.Core.Infrastructure.Authorization;
using DataReef.TM.ClientApi.Models;
using DataReef.TM.Contracts.Services;
using DataReef.TM.Models.DTOs.Signatures;

namespace DataReef.TM.ClientApi.Controllers
{
    /// <summary>
    /// Dispositions
    /// </summary>
    [RoutePrefix("dispositions")]
    public class DispositionsController : ApiController
    {
        private readonly IOUService _ouService;
        private readonly IInquiryService _inquiryService;

        public DispositionsController(IOUService ouService, IInquiryService inquiryService)
        {
            _ouService = ouService;
            _inquiryService = inquiryService;
        }

        /// <summary>
        /// Get all dispositions
        /// </summary>
        /// <param name="ouid">The organization id</param>
        /// <param name="startDate">Optional start date filter</param>
        /// <param name="itemsPerPage">Items per page, default 100</param>
        /// <param name="endDate">Optional end date filter</param>
        /// <param name="pageNumber">Page number, default 1</param>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        [ResponseType(typeof(GenericResponse<List<DispositionDataView>>))]
        public IHttpActionResult GetDispositions(Guid ouid, System.DateTime? startDate = null, int? itemsPerPage = 100, System.DateTime? endDate = null, int pageNumber = 1)
        {
            var guidList = new List<Guid>() { SmartPrincipal.OuId };
            var ouGuids = _ouService.GetHierarchicalOrganizationGuids(guidList);

            if (!ouGuids.Contains(ouid))
            {
                return NotFound();
            }

            int itemCount = itemsPerPage.HasValue ? itemsPerPage.Value : 100;

            string filter = $"OUID={ouid}";
            if (startDate.HasValue)
                filter = $"{filter}&DateCreated>{startDate.Value.Date}";
            if (endDate.HasValue)
                filter = $"{filter}&DateCreated<{endDate.Value.Date}";

            var dispositions = _inquiryService.List(pageNumber: pageNumber, itemsPerPage: itemCount, filter: filter, include:"Property").ToList(); //, include: "Property,User.Person"
            var dispositionsResponse = dispositions.Select(d => new DispositionDataView(d)).ToList();

            var ret = new GenericResponse<List<DispositionDataView>>(dispositionsResponse);

            return Ok(ret);
        }
    }
}