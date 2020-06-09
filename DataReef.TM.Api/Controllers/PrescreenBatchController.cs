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
    [RoutePrefix("api/v1/prescreenbatches")]
    public class PrescreenBatchesController : EntityCrudController<PrescreenBatch>
    {
        private readonly IPrescreenBatchService batchService;

        public PrescreenBatchesController(IPrescreenBatchService batchService, ILogger logger)
            : base(batchService, logger)
        {
            this.batchService = batchService;
        }

        public override async Task<PrescreenBatch> Patch(System.Web.Http.OData.Delta<PrescreenBatch> item)
        {
            throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotImplemented) { Content = new StringContent("Method not implemented") });
    
        }

        public override async Task<ICollection<PrescreenBatch>> PostMany(List<PrescreenBatch> items)
        {
            throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotImplemented) { Content = new StringContent("Method not implemented") });
        }

        public override async Task<HttpResponseMessage> DeleteByGuid(Guid guid)
        {
            throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotImplemented) { Content = new StringContent("Method not implemented") });
        }

        public override async Task<HttpResponseMessage> Delete(PrescreenBatch item)
        {
            throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotImplemented) { Content = new StringContent("Method not implemented") });   
        }
    
        public override async Task<PrescreenBatch> Put(PrescreenBatch item)
        {
            throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotImplemented) { Content = new StringContent("Method not implemented") });
        }

        public override async Task<PrescreenBatch> Post(PrescreenBatch item)
        {
            return batchService.Insert(item);         
        }
    }
}
