using DataReef.Core.Infrastructure.Authorization;
using DataReef.Core.Logging;
using DataReef.TM.Api.Classes.Requests;
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
using DataReef.Auth.Helpers;
using System.Threading.Tasks;

namespace DataReef.TM.Api.Controllers
{
    /// <summary>
    /// Managers CRUD for Properties
    /// </summary>
    [RoutePrefix("api/v1/notifications")]
    public class NotificationsController : EntityCrudController<Notification>
    {
        private readonly INotificationService _notificationService;

        public NotificationsController(INotificationService notificationService,
                                    ILogger logger)
            : base(notificationService, logger)
        {
            this._notificationService = notificationService;
        }



        /// <summary>
        /// Gets all notifications for user
        /// </summary>
        /// <param name="personID"></param>
        /// <param name="status"></param>
        /// <param name="pageNumber"></param>
        /// <param name="itemsPerPage"></param>
        /// <returns></returns>
        [Route("user/{personID}")]
        [ResponseType(typeof(IEnumerable<Notification>))]
        [HttpGet]
        public async Task<IHttpActionResult> GetAllForProperty(Guid personID, IgniteNotificationSeenStatus? status = null, int pageNumber = 0, int itemsPerPage = 20)
        {
            var result = _notificationService.GetNotificationsForPerson(personID, status, pageNumber, itemsPerPage);

            return Ok(result);
        }

        /// <summary>
        /// Mark all notifications for user as seen
        /// </summary>
        /// <param name="personID"></param>
        /// <returns></returns>
        [Route("user/{personID}/read")]
        [ResponseType(typeof(IEnumerable<Notification>))]
        [HttpPost]
        public async Task<IHttpActionResult> MarkAllUserNotificationsAsRead(Guid personID)
        {
            var result = _notificationService.MarkAllNotificationsAsRead(personID);

            return Ok(result);
        }

        /// <summary>
        /// Mark Notification as read
        /// </summary>
        /// <param name="notificationID"></param>
        /// <returns></returns>
        [Route("{notificationID}/read")]
        [ResponseType(typeof(Notification))]
        [HttpPost]
        public async Task<IHttpActionResult> MarkNotificationAsRead(Guid notificationID)
        {
            var result = _notificationService.MarkAsRead(notificationID);

            return Ok(result);
        }

        /// <summary>
        /// Count unread notifications for user
        /// </summary>
        /// <param name="personID"></param>
        /// <returns></returns>
        [Route("user/{personID}/count")]
        [ResponseType(typeof(GenericResponse<int>))]
        [HttpGet]
        public async Task<IHttpActionResult> CountUnreadForPerson(Guid personID)
        {
            var result = _notificationService.CountUnreadNotifications(personID);

            return Ok(new GenericResponse<int> { Response = result });
        }

        /// <summary>
        /// Dismiss Notification from smartboard
        /// </summary>
        /// <param name="notificationID"></param>
        /// <param name="apiKey"></param>
        /// <returns></returns>
        [Route("sb/{notificationID}/dismiss/{apiKey}")]
        [AllowAnonymous, InjectAuthPrincipal]
        [ResponseType(typeof(bool))]
        [HttpPost]
        public async Task<IHttpActionResult> MarkNotificationAsReadFromSmartboard(string apiKey, string notificationID)
        {

            bool checkTime = CryptographyHelper.checkTime(apiKey);
            string DecyptApiKey = CryptographyHelper.getDecryptAPIKey(apiKey);

            var result = _notificationService.MarkAsReadFromSmartboard(notificationID, DecyptApiKey);

            return Ok(result);
        }
    }
}
