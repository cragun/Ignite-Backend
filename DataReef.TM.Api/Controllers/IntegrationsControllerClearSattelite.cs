using DataReef.Integrations.Agemni;
using DataReef.TM.Api.Classes;
using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DataReef.TM.Api.Controllers
{
	public partial class IntegrationsController : ApiController
	{
		[HttpGet]
        [Route("ClearSatellite/Agemni/lookupdata")]
		public AgemniLookupData GetAgemniLookupData()
		{
			DataReef.Integrations.Agemni.AgemniLookupData lookupData = new AgemniLookupData();
			return lookupData;
		}

		[HttpPost]
        [Route("ClearSatellite/Agemni/account")]
		public HttpResponseMessage PerformAgemniIntegration(DataReef.Integrations.Agemni.IntegrationRequest request)
		{
			try
			{
				DataReef.Integrations.Agemni.AgemniIntegrationProvider provider = new AgemniIntegrationProvider();
				bool success                                                    = provider.Integrate(request);

				var response = Request.CreateResponse(
					success ? HttpStatusCode.OK : HttpStatusCode.BadRequest,
					new { Message = provider.ErrorMessage.ToString() });

				if (provider.DebugInfo.Length > 0)
				{
					response.AddDtvDebugInfoHeader(provider.DebugInfo.ToString());
				}

				return response;
			}
			catch (Exception e)
			{
				return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
			}
		}
    }
}