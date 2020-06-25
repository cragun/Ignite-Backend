using DataReef.Core.Logging;
using DataReef.TM.Contracts.Services;
using DataReef.TM.Models;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Description;
using System.Threading.Tasks;

namespace DataReef.TM.Api.Controllers
{
    [RoutePrefix("api/v1/assignments")]
    public class AssignmentsController : EntityCrudController<Assignment>
    {
        private IAssignmentService _assignmentService;

        public AssignmentsController(IAssignmentService assignmentService, ILogger logger) : base(assignmentService, logger)
        {
            _assignmentService = assignmentService;
        }

        [HttpPost]
        [Route("validate/people")]
        [ResponseType(typeof(List<KeyValuePair<Guid, string>>))]
        public async Task<IHttpActionResult> ValidatePeopleTerritories(ValidationRequest req)
        {
            var response = _assignmentService.ValidatePeopleOUs(req.Data.Select(d => new KeyValuePair<Guid, Guid>(d.PersonId, d.OUID)).ToList());
            return Ok(response);
        }
    }

    public class ValidationRequest
    {
        public List<PersonTerritory> Data { get; set; }
    }

    public class PersonTerritory
    {
        public Guid PersonId { get; set; }
        public Guid OUID { get; set; }
    }
}
