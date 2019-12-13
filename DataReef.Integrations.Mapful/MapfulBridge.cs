using DataReef.Core.Attributes;
using DataReef.Integrations.Mapful.DataViews;
using RestSharp;
using System.Collections.Generic;
using System.Configuration;
using System.ServiceModel;
using System.ServiceModel.Activation;

namespace DataReef.Integrations.Mapful
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(AddressFilterMode = AddressFilterMode.Any)]
    [Service(typeof(IMapfulBridge))]
    public class MapfulBridge : IMapfulBridge
    {
        private static string _baseUrl = ConfigurationManager.AppSettings["DataReef.Mapful.Url"];

        public ICollection<LeadDataView> GetLeads(LeadsRequest request)
        {
            var req = new RestRequest("api/data/leads", Method.POST);
            req.AddHeader("Content-Type", "application/json");
            req.AddJsonBody(request);
            req.AddDataReefAuthHeader();

            var client = new RestClient(_baseUrl);
            var resp = client.Execute<List<LeadDataView>>(req);

            if (resp.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return resp.Data;
            }

            return new List<LeadDataView>();
        }

        public int CountProperties(AnalyzeRequest request)
        {
            var req = new RestRequest("api/layers/analyze/simple", Method.POST);
            req.AddHeader("Content-Type", "application/json");
            req.AddJsonBody(request);
            req.AddDataReefAuthHeader();

            var client = new RestClient(_baseUrl);
            var resp = client.Execute<AnalyzeResponse>(req);

            if (resp.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return resp.Data?.Total ?? 0;
            }

            return 0;
        }

    }
}
