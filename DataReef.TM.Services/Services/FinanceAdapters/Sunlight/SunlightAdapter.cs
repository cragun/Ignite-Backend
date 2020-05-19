using DataReef.Core.Attributes;
using DataReef.TM.Contracts.Services;
using DataReef.TM.Contracts.Services.FinanceAdapters;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Text;
using System.Threading.Tasks;


namespace DataReef.TM.Services.Services.FinanceAdapters.Sunlight
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(AddressFilterMode = AddressFilterMode.Any)]
    [Service(typeof(ISunlightAdapter))]
    public class SunlightAdapter: ISunlightAdapter
    {
        private static readonly string url = System.Configuration.ConfigurationManager.AppSettings["Sunlight.test.url"];
        private static readonly string AuthUsername = System.Configuration.ConfigurationManager.AppSettings["Sunlight.Auth.Username"];
        private static readonly string AuthPassword = System.Configuration.ConfigurationManager.AppSettings["Sunlight.Auth.Password"];
        private static readonly string Username = System.Configuration.ConfigurationManager.AppSettings["Sunlight.Username"];
        private static readonly string Password = System.Configuration.ConfigurationManager.AppSettings["Sunlight.Password"];

        private RestClient client
        {
            get
            {
                return new RestClient(url);
            }
        }

        public class TokenCredentials
        {
            public string username { get; set; }
            public string password { get; set; }
        }

        public string GetSunlightToken()
        {
            try
            {
                string svcCredentials = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(AuthUsername + ":" + AuthPassword));

                var cred = new TokenCredentials();
                cred.username = Username;
                cred.password = Password;

                var request = new RestRequest($"/gettoken/accesstoken", Method.POST);
                request.AddJsonBody(cred);
                request.AddHeader("Authorization", "Basic " + svcCredentials);
                var response = client.Execute(request);

                var requestString = request == null ? null : JsonConvert.SerializeObject(request);
                var responseString = response == null ? null : JsonConvert.SerializeObject(response);

                return requestString + "request:" + responseString;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

        }

    }
}
