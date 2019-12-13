using DataReef.Core.Attributes;
using DataReef.Integrations.Sunnova.Interfaces;
using DataReef.Integrations.Sunnova.Models.Request;
using OAuth2.Client.Impl;
using OAuth2.Configuration;
using OAuth2.Infrastructure;
using RestSharp;
using System.ServiceModel;
using System.ServiceModel.Activation;

namespace DataReef.Integrations.Sunnova.Services
{

    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(AddressFilterMode = AddressFilterMode.Any)]
    [Service(typeof(ISunnovaService))]
    public class SunnovaService : ISunnovaService
    {

        private static string BaseURLConfigName = "Integrations.Sunnova.BaseURL";
        private static string BaseURL = System.Configuration.ConfigurationManager.AppSettings[BaseURLConfigName];

        private RestClient _client;
        private RestClient Client
        {
            get
            {

                if (_client == null)
                {
                    _client = new RestClient(BaseURL);
                }
                return _client;
            }
        }

        public void CreateLead(CreateLeadRequest lead)
        {
            var client = Client;
            var req = new RestRequest("/partner/leads", Method.POST);
            // TODO: add the authorization header token
            req.AddJsonBody(lead);
            var resp = client.Execute(req);
        }

        private void Login()
        {
            var config = new ClientConfiguration();
            var client = new SalesforceClient(new RequestFactory(), config);
            var token = client.GetCurrentToken();
        }
    }
}
