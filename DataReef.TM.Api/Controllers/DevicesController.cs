using DataReef.Core.Classes;
using DataReef.Core.Infrastructure.Authorization;
using DataReef.Core.Logging;
using DataReef.TM.Api.Classes.Requests;
using DataReef.TM.Contracts.Services;
using DataReef.TM.Models;
using DataReef.TM.Models.PushNotifications;
using System;
using System.Web.Http;
using System.Web.Http.Description;

namespace DataReef.TM.Api.Controllers
{
    [RoutePrefix("api/v1/devices")]
    public class DevicesController : EntityCrudController<Device>
    {
        IDeviceService _service;
        private readonly Lazy<IDataService<PushSubscription>> _pushSubscriptionService;

        public DevicesController(IDeviceService service,
            ILogger logger,
            Lazy<IDataService<PushSubscription>> pushSubscriptionService)
            : base(service, logger)
        {
            _service = service;
            _pushSubscriptionService = pushSubscriptionService;
        }

        [HttpGet]
        [Route("validate")]
        [ResponseType(typeof(bool))]
        public IHttpActionResult Validate()
        {
            var valid = _service.Validate();

            if (valid)
            {
                return Ok();
            }

            return Unauthorized();
        }

        /// <summary>
        /// Register the APN device token
        /// </summary>
        /// <param name="request">Generic request that contains the APN Device Token </param>
        /// <returns></returns>
        [HttpPost]
        [Route("APNtoken/register")]
        public IHttpActionResult RegisterAPNToken(GenericRequest<string> request)
        {
            _service.RegisterAPNDeviceToken(request.Request);
            return Ok();
        }

        /// <summary>
        /// Unregister current device from APN.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("APNtoken/unregister")]
        public IHttpActionResult UnregisterAPNToken()
        {
            _service.UnRegisterAPNDeviceToken();
            return Ok();
        }

        /// <summary>
        /// Subscribe to receive notifications when someting changes for given entity type.
        /// </summary>
        /// <param name="entity">Name of the entity (e.g. Territory, Property, OU, ... )</param>
        /// <param name="entityID">The Guid of the entity to watch for changes for</param>
        /// <param name="notificationType">Type of notification. Regular or Silent.</param>
        /// <returns></returns>
        [HttpPost]
        [Route("push/subscribe/{entity}/{entityID}/{notificationType}")]
        [ResponseType(typeof(PushSubscription))]
        public IHttpActionResult SubscribeToPushNotification(string entity, Guid entityID, NotificationType notificationType = NotificationType.Silent)
        {
            var subscription = new PushSubscription
            {
                Name = entity,
                ExternalID = entityID.ToString(),
                NotificationType = notificationType,
                DeviceId = SmartPrincipal.DeviceId
            };
            _pushSubscriptionService.Value.Insert(subscription);
            return Ok(subscription);
        }

        [HttpPost]
        [Route("push/unsubscribe/{subscriptionID:guid}")]
        [ResponseType(typeof(SaveResult))]
        public IHttpActionResult UnsubscribeToPushNotification(Guid subscriptionID)
        {
            var result = _pushSubscriptionService.Value.Delete(subscriptionID);
            return Ok(result);
        }

    }
}
