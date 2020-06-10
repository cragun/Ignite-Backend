using System;
using System.Collections.Generic;
using DataReef.Core.Logging;
using DataReef.TM.Contracts.Services;
using DataReef.TM.Models;
using System.Web.Http;
using System.Net;
using System.Net.Http;
using System.Linq;
using System.IO;
using System.Net.Http.Headers;
using System.Web.Http.Description;
using DataReef.TM.Models.Layers;
using DataReef.Core.Infrastructure.Authorization;
using System.Threading.Tasks;

namespace DataReef.TM.Api.Controllers
{
    /// <summary>
    /// Managers CRUD for Person
    /// </summary>
    [RoutePrefix("api/v1/layers")]
    public class LayersController : EntityCrudController<Layer>
    {
        private readonly ILayerService layerService;

        public LayersController(ILayerService layerService, ILogger logger)
            : base(layerService, logger)
        {
            this.layerService = layerService;
        }

        [HttpGet]
        [Route("ous/{ouid:guid}")]
        public async Task<ICollection<Layer>>  ListByOUID(Guid ouid)
        {
            return this.layerService.GetLayersForOU(ouid, false);
        }

       
        public override async Task<ICollection<Layer>> PostMany(List<Layer> items)
        {
            throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.MethodNotAllowed)
            {
                Content = new StringContent("Method Not Available")
            });
        }
        public override async Task<Layer> Post(Layer item)
        {
            throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.MethodNotAllowed)
            {
                Content = new StringContent("Method Not Available")
            });
        }
        public override async Task<HttpResponseMessage> DeleteByGuid(Guid guid)
        {
            throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.MethodNotAllowed)
            {
                Content = new StringContent("Method Not Available")
            });
        }

        public override async Task<Layer> Put(Layer item)
        {
            throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.MethodNotAllowed)
            {
                Content = new StringContent("Method Not Available")
            });
        }
        public override async Task<HttpResponseMessage> Delete(Layer item)
        {
            throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.MethodNotAllowed)
            {
                Content = new StringContent("Method Not Available")
            });
        }
        public override async Task<Layer> Get(Guid guid, string include = "", string exclude = "", string fields = "", bool deletedItems = false)
        {
            throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.MethodNotAllowed)
            {
                Content = new StringContent("Method Not Available")
            });
        }
       

    }
}
