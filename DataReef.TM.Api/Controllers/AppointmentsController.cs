using DataReef.Core.Logging;
using DataReef.TM.Contracts.Services;
using DataReef.TM.Models;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Description;
using DataReef.TM.Api.Classes.Requests;
using DataReef.TM.Models.Enums;
using DataReef.Core.Infrastructure.Authorization;
using DataReef.TM.Models.DTOs.SmartBoard;
using DataReef.Auth.Helpers;
using DataReef.TM.Models.DataViews;

namespace DataReef.TM.Api.Controllers
{
    [RoutePrefix("api/v1/appointments")]
    public class AppointmentsController : EntityCrudController<Appointment>
    {
        private IAppointmentService _appointmentService;

        public AppointmentsController(IAppointmentService appointmentService, ILogger logger) : base(appointmentService, logger)
        {
            _appointmentService = appointmentService;
        }

        [HttpPost]
        [Route("{appointmentID:guid}/status")]
        [ResponseType(typeof(Appointment))]
        public IHttpActionResult ChangeAppointmentStatus(Guid appointmentID, [FromBody] GenericRequest<AppointmentStatus> request)
        {
            var response = _appointmentService.SetAppointmentStatus(appointmentID, request.Request);
            return Ok(response);
        }

        /// <summary>
        /// Get method used by SmartBoard to retrieve appointments for the specified property
        /// </summary>
        /// <param name="propertyID"></param>
        /// <param name="apiKey"></param>
        [HttpGet, Route("sb/lead/{propertyID}/{apiKey}")]
        [ResponseType(typeof(IEnumerable<SBAppointmentDTO>))]
        [AllowAnonymous, InjectAuthPrincipal]
        public IEnumerable<SBAppointmentDTO> GetAppointmentsForProperty(long propertyID, string apiKey)
        {
            return _appointmentService.GetSmartboardAppointmentsForPropertyID(propertyID, apiKey);
        }

        /// <summary>
        /// Get method used by SmartBoard to retrieve appointments all properties linked to a specified ou
        /// </summary>
        /// <param name="ouID">the ID for the OU </param>
        /// <param name="apiKey"></param>
        /// <param name="pageNumber"></param>
        /// <param name="itemsPerPage"></param>
        [HttpGet, Route("sb/ou/{ouID}/{apiKey}")]
        [ResponseType(typeof(IEnumerable<SBAppointmentDTO>))]
        [AllowAnonymous, InjectAuthPrincipal]
        public IEnumerable<SBAppointmentDTO> GetAppointmentsForOU(Guid ouID, string apiKey, int pageNumber = 1, int itemsPerPage = 20)
        {
            return _appointmentService.GetSmartboardAppointmentsForOUID(ouID, pageNumber, itemsPerPage, apiKey);
        }


        /// <summary>
        /// POST method used by SmartBoard to retrieve appointments for the specified OUID per PersonId
        /// </summary>
        /// <param name="request"> </param>
        /// <param name="ouID">the ID for the OU </param>
        /// <param name="apiKey"></param>
        /// <param name="date"></param>
        [HttpPost]
        [Route("sb/ouAppointment/{ouID}/{apiKey}/{date}")]
        public IHttpActionResult GetMembers([FromBody]OUMembersRequest request, Guid ouID, string apiKey, string date)
        {
            bool checkTime = CryptographyHelper.checkTime(apiKey);
            string DecyptApiKey = CryptographyHelper.getDecryptAPIKey(apiKey);
            var data =  _appointmentService.GetMembersWithAppointment(request, ouID, DecyptApiKey, DateTime.Parse(date));
            return Ok(data);
        }

        /// <summary>
        /// Get method used by SmartBoard to retrieve appointments for the specified property
        /// </summary>
        /// <param name="smartboardUserId"></param>
        /// <param name="apiKey"></param>
        [HttpGet, Route("sb/user/{smartboardUserId}/{apiKey}")]
        [ResponseType(typeof(IEnumerable<SBAppointmentDTO>))]
        [AllowAnonymous, InjectAuthPrincipal]
        public IEnumerable<SBAppointmentDTO> GetAppointmentsAssignedToSmartboardUser(string smartboardUserId, string apiKey)
        {
            return _appointmentService.GetSmartboardAppointmentsAssignedToUserId(smartboardUserId, apiKey);
        }

        /// <summary>
        /// POST method used by SmartBoard to create a new appointment for a specified property
        /// </summary>
        /// <param name="request"></param>
        /// <param name="apiKey"></param>
        [HttpPost, Route("sb/{apiKey}")]
        [ResponseType(typeof(SBAppointmentDTO))]
        [AllowAnonymous, InjectAuthPrincipal]
        public SBAppointmentDTO CreateNewAppointmentFromSmartBoard(SBCreateAppointmentRequest request, string apiKey)
        {
            bool checkTime = CryptographyHelper.checkTime(apiKey);
            string DecyptApiKey = CryptographyHelper.getDecryptAPIKey(apiKey);

            return _appointmentService.InsertNewAppointment(request, DecyptApiKey);
        }

        /// <summary>
        /// PATCH method used by SmartBoard to edit an appointment for a specified property
        /// </summary>
        /// <param name="request"></param>
        /// <param name="appointmentID"></param>
        /// <param name="apiKey"></param>
        [HttpPatch, Route("sb/{appointmentID}/{apiKey}")]
        [ResponseType(typeof(SBAppointmentDTO))]
        [AllowAnonymous, InjectAuthPrincipal]
        public SBAppointmentDTO EditAppointmentFromSmartBoard(SBEditAppointmentRequest request, Guid appointmentID, string apiKey)
        {
            return _appointmentService.EditSBAppointment(request, appointmentID, apiKey);
        }


        /// <summary>
        /// DELETE method used by SmartBoard to remove the specified appointment from the system
        /// </summary>
        /// <param name="appointmentID"></param>
        /// <param name="apiKey"></param>
        [HttpDelete, Route("sb/{appointmentID}/{apiKey}")]
        [ResponseType(typeof(bool))]
        [AllowAnonymous, InjectAuthPrincipal]
        public bool RemoveAppointmentSmartBoard(Guid appointmentID, string apiKey)
        {
            return _appointmentService.DeleteSBAppointment(appointmentID, apiKey);
        }

        /// <summary>
        /// POST method used by SmartBoard to create a new appointment for a specified property
        /// </summary>
        /// <param name="request"></param>
        /// <param name="apiKey"></param>
        [HttpPost, Route("sb/status/{apiKey}")]
        [ResponseType(typeof(SBAppointmentDTO))]
        [AllowAnonymous, InjectAuthPrincipal]
        public SBAppointmentDTO SetAppointmentStatusSmartBoard(SBSetAppointmentStatusRequest request, string apiKey)
        {
            return _appointmentService.SetAppointmentStatusFromSmartboard(request, apiKey);
        }
    }

}
