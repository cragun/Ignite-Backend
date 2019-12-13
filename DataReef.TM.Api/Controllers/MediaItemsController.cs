using System;
using System.Collections.Generic;
using DataReef.Core.Logging;
using DataReef.TM.Contracts.Services;
using DataReef.TM.Models;
using System.Web.Http;
using System.Net;
using System.Net.Http;
using System.IO;
using System.Net.Http.Headers;


namespace DataReef.TM.Api.Controllers
{
    [RoutePrefix("api/v1/mediaitems")]
    public class MediaItemsController : EntityCrudController<MediaItem>
    {
        private readonly IMediaItemService service;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="service"></param>
        /// <param name="logger"></param>
        public MediaItemsController(IMediaItemService service, ILogger logger)
            : base(service, logger)
        {
            this.service = service;
        }

        [HttpGet]
        [Route("ou/{ouid:guid}")]
        public IHttpActionResult GetForOU(Guid ouid, bool deletedItems = false, MediaType? mediaType = null)
        {
            var mType = mediaType.HasValue ? (int)mediaType.Value : -1;

            return Ok(service.GetMediaItems(ouid, deletedItems, mType));
        }


    }
}