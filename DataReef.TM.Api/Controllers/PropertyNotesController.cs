using DataReef.Core.Infrastructure.Authorization;
using DataReef.Core.Logging;
using DataReef.TM.Contracts.Services;
using DataReef.TM.Models;
using DataReef.TM.Models.DTOs;
using DataReef.TM.Models.DTOs.Properties;
using DataReef.TM.Models.DTOs.PropertyAttachments;
using DataReef.TM.Models.Solar;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using WebApi.OutputCache.V2;

namespace DataReef.TM.Api.Controllers
{
    /// <summary>
    /// Managers CRUD for Properties
    /// </summary>
    [RoutePrefix("api/v1/propertynotes")]
    public class PropertyNotesController : EntityCrudController<PropertyNote>
    {
        private readonly IPropertyNoteService _propertyNoteService;

        public PropertyNotesController(IPropertyNoteService propertyNoteService,
                                    ILogger logger)
            : base(propertyNoteService, logger)
        {
            this._propertyNoteService = propertyNoteService;
        }


        /// <summary>
        /// Gets all PropertyNotes found for the specified propertyID
        /// </summary>
        /// <param name="propertyID"></param>
        /// <returns></returns>
        [Route("property/{propertyID}")]
        [ResponseType(typeof(IEnumerable<PropertyNote>))]
        [HttpGet]
        public IHttpActionResult GetAllForProperty(Guid propertyID)
        {
            var result = _propertyNoteService.GetNotesByPropertyID(propertyID);

            return Ok(result);
        }

        /// <summary>
        /// Finds all users with access to the property by partial match on email and name
        /// </summary>
        /// <param name="propertyID"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [Route("property/{propertyID}/users/query")]
        [ResponseType(typeof(IEnumerable<PropertyNote>))]
        [HttpPost]
        public IHttpActionResult QueryUsersForProperty(Guid propertyID, [FromBody]PropertyNoteUserQueryRequest request)
        {
            var result = _propertyNoteService.QueryForPerson(propertyID, request?.Email, request?.Name);

            return Ok(result);
        }

        /// <summary>
        /// / Gets the notes linked to the specified smartboard lead Id
        /// </summary>
        /// <param name="leadId"></param>
        /// <param name="apiKey"></param>
        /// <param name="igniteId"></param>
        /// <returns></returns>
        [Route("sb/{leadId}/{apiKey}")]
        [ResponseType(typeof(ICollection<SBNoteDTO>))]
        [HttpGet]
        [AllowAnonymous, InjectAuthPrincipal]
        public IHttpActionResult GetAllForSmartboard(long leadId, string apiKey, long? igniteId)
        {
            var result = _propertyNoteService.GetAllNotesForProperty(leadId, igniteId, apiKey);

            return Ok(result);
        }


        /// <summary>
        /// creates a new note from smartboard
        /// </summary>
        /// <param name="request"></param>
        /// <param name="apiKey"></param>
        /// <returns></returns>
        [Route("sb/{apiKey}")]
        [ResponseType(typeof(SBNoteDTO))]
        [HttpPost]
        [AllowAnonymous, InjectAuthPrincipal]
        public IHttpActionResult CreateNoteFromSmartboard([FromBody]SBNoteDTO request, string apiKey)
        {
            var result = _propertyNoteService.AddNoteFromSmartboard(request, apiKey);

            return Ok(result);
        }


        /// <summary>
        /// edits a note from smartboard
        /// </summary>
        /// <param name="request"></param>
        /// <param name="apiKey"></param>
        /// <returns></returns>
        [Route("sb/{apiKey}")]
        [ResponseType(typeof(SBNoteDTO))]
        [HttpPatch]
        [AllowAnonymous, InjectAuthPrincipal]
        public IHttpActionResult EditNoteFromSmartboard([FromBody]SBNoteDTO request, string apiKey)
        {
            var result = _propertyNoteService.EditNoteFromSmartboard(request, apiKey);

            return Ok(result);
        }

        /// <summary>
        /// deletes a note by its specified id
        /// </summary>
        /// <param name="noteId"></param>
        /// <param name="userID"></param>
        /// <param name="apiKey"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        [Route("sb/{leadId}/{userID}/{apiKey}")]
        [HttpDelete]
        [AllowAnonymous, InjectAuthPrincipal]
        public IHttpActionResult DeleteNoteFromSmartboard(Guid noteId, string userID, string apiKey, string email)
        {
            _propertyNoteService.DeleteNoteFromSmartboard(noteId, userID, apiKey, email);

            return Ok();
        }



        /// <summary>
        /// / Gets Territories where Lead can transfer using SB apikey
        /// </summary>
        /// <param name="leadId"></param>
        /// <param name="apiKey"></param>
        /// <returns></returns>
        [Route("sb/Territories/{leadId}/{apiKey}")]
        [HttpGet]
        [AllowAnonymous, InjectAuthPrincipal]
        public IHttpActionResult CheckCanTransferLead(long leadId, string apiKey)
        {
            var result = _propertyNoteService.GetTerritoriesList(leadId, apiKey);
            return Ok(result);
        }



        ///// <summary>
        ///// / Update TerritoryId for Property which given By SB.
        ///// </summary>
        ///// <param name="leadId"></param>
        ///// <param name="TerritoryId"></param>
        ///// <param name="email"></param>
        ///// <param name="apiKey"></param>
        ///// <returns></returns>
        //[Route("sb/Transfer/{leadId}/{TerritoryId}/{apiKey}")]
        //[HttpPost]
        //[AllowAnonymous, InjectAuthPrincipal]
        //public IHttpActionResult UpdateTerritoryIdInProperty(long leadId, long TerritoryId, string apiKey, string email)
        //{
        //    var result = _propertyNoteService.UpdateTerritoryIdInProperty(leadId, TerritoryId, apiKey, email);
        //    return Ok(result);
        //}



    }
}
