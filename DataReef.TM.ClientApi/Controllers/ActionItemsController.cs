using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using DataReef.Core.Infrastructure.Authorization;
using DataReef.TM.ClientApi.Models;
using DataReef.TM.Contracts.Services;
using DataReef.TM.Models.DTOs.EPC;
using ActionItemDataView = DataReef.TM.ClientApi.Models.ActionItemDataView;

namespace DataReef.TM.ClientApi.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [RoutePrefix("actionitems")]
    public class ActionItemsController : ApiController
    {
        private readonly Func<IActionItemService> _actionItemServiceFactory;

        public ActionItemsController(Func<IActionItemService> actionItemServiceFactory)
        {
            _actionItemServiceFactory = actionItemServiceFactory;
        }

        [HttpGet]
        [Route("properties/{propertyID:guid}")]
        public IHttpActionResult GetPropertyActionItems(Guid propertyID)
        {
            var actionItems = _actionItemServiceFactory()
                .List(itemsPerPage: int.MaxValue, filter: $"PropertyID={propertyID}", include: "Property,Person.PhoneNumbers,Person.User")
                .Select(i => new ActionItemDataView(i))
                .ToList();

            return Ok(new GenericResponse<List<ActionItemDataView>>(actionItems));
        }

        [HttpGet]
        [Route("people/{personID:guid}")]
        public IHttpActionResult GetPersonActionItems(Guid personID)
        {
            var actionItems = _actionItemServiceFactory()
                .List(itemsPerPage: int.MaxValue, filter: $"PersonID={personID}", include: "Property,Person.PhoneNumbers,Person.User")
                .Select(i => new ActionItemDataView(i))
                .ToList();

            return Ok(new GenericResponse<List<ActionItemDataView>>(actionItems));
        }

        [HttpPost]
        [Route("")]
        public IHttpActionResult UploadActionItems([FromBody]List<ActionItemInput> actionItems)
        {
            if (actionItems == null)
                return BadRequest($"Invalid {nameof(actionItems)}");

            var ouid = SmartPrincipal.OuId;

            _actionItemServiceFactory().UploadActionItems(ouid, actionItems);

            return Ok();
        }
    }
}
