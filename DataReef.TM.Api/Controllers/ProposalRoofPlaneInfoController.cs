using DataReef.Core.Logging;
using DataReef.TM.Api.Classes.Requests;
using DataReef.TM.Contracts.Services;
using DataReef.TM.Models.Solar;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace DataReef.TM.Api.Controllers
{

    [RoutePrefix("api/v1/ProposalRoofPlane")]
    public class ProposalRoofPlaneInfoController : EntityCrudController<ProposalRoofPlaneInfo>
    {
        
        public ProposalRoofPlaneInfoController(IDataService<ProposalRoofPlaneInfo> dataService, ILogger logger) : base(dataService, logger)
        {
            
        }

       

        #region Forbidden methods

        public override async Task<HttpResponseMessage> Delete(ProposalRoofPlaneInfo item)
        {
            throw new HttpResponseException(HttpStatusCode.Forbidden);
        }

        public override async Task<HttpResponseMessage> DeleteByGuid(Guid guid)
        {
            throw new HttpResponseException(HttpStatusCode.Forbidden);
        }

        public override async Task<HttpResponseMessage> DeleteMany([FromBody] IDsListWrapperRequest req)
        {
            throw new HttpResponseException(HttpStatusCode.Forbidden);
        }

        #endregion
    }
}
