using DataReef.Core.Logging;
using DataReef.TM.Contracts.Services;
using DataReef.TM.Models.Credit;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace DataReef.TM.Api.Controllers
{
    /// <summary>
    /// Allows to to create and list Batch Prescreens
    /// </summary>
    [RoutePrefix("api/v1/prescreeninstants")]
    public class PrescreenInstantsController : EntityCrudController<PrescreenInstant>
    {
        private readonly IPrescreenInstantService instantprescreenService;

        public PrescreenInstantsController(IPrescreenInstantService instantprescreenService, ILogger logger)
            : base(instantprescreenService, logger)
        {
            this.instantprescreenService = instantprescreenService;
        }

        public override async Task<PrescreenInstant> Patch(System.Web.Http.OData.Delta<PrescreenInstant> item)
        {
            throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotImplemented) { Content = new StringContent("Method not implemented") });
        }

        public override async Task<ICollection<PrescreenInstant>> PostMany(ICollection<PrescreenInstant> items)
        {
            throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotImplemented) { Content = new StringContent("Method not implemented") });
        }

        public override async Task<HttpResponseMessage> DeleteByGuid(Guid guid)
        {
            throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotImplemented) { Content = new StringContent("Method not implemented") });
        }

        public override async Task<HttpResponseMessage> Delete(PrescreenInstant item)
        {
            throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotImplemented) { Content = new StringContent("Method not implemented") });   
        }

        public override async Task<PrescreenInstant> Put(PrescreenInstant item)
        {
            throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotImplemented) { Content = new StringContent("Method not implemented") });
        }

        public override async Task<PrescreenInstant> Post(PrescreenInstant item)
        {
            return instantprescreenService.Insert(item);         
        }
    }
}
