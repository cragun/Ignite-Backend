using DataReef.Core.Attributes;
using DataReef.Integrations.Core;
using DataReef.Integrations.Core.Infrastructure;
using DataReef.Integrations.LoanPal.Models;
using DataReef.Integrations.LoanPal.Models.LoanPal;
using RestSharp;
using RestSharp.Serializers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.Integrations.LoanPal
{

    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(AddressFilterMode = AddressFilterMode.Any)]
    [Service(typeof(ILoanPalBridge))]
    public class LoanPalBridge : BaseBridge, ILoanPalBridge
    {
        private static readonly string Application_AccessKey = ConfigurationManager.AppSettings["LoanPal.Loan.Application.Credentials.AccessKey"];
        private static readonly string Application_SecretKey = ConfigurationManager.AppSettings["LoanPal.Loan.Application.Credentials.SecretKey"];
        private static readonly string Application_Region = ConfigurationManager.AppSettings["LoanPal.Loan.Application.Credentials.Region"];
        private static readonly string Application_ApiKey = ConfigurationManager.AppSettings["LoanPal.Loan.Application.Credentials.ApiKey"];

        public LoanPalBridge()
        {
            BaseUrl = ConfigurationManager.AppSettings["LoanPal.Loan.Calculator.BaseURL"];
        }

        public LoanCalculatorResponse CalculateLoan(LoanCalculatorRequest request)
        {
            var queryString = request.ToQueryString(toLowerValues: true);
            //var req = new RestRequest($"solciussb/restapi/v1/loanCalc?{queryString}", Method.GET);
            var req = new RestRequest($"/loanCalc?{queryString}", Method.GET);
            req.RequestFormat = DataFormat.Json;
            req.JsonSerializer = new RestSharpJsonSerializer();

            var response = Client.Execute<LoanCalculatorResponse>(req);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var resp = response.Data;
                resp.ProcessData();

                return resp;
            }
            return null;
        }


        public ApplicationResponse SubmitApplication(ApplicationRequest request)
        {
            var client = Client;
            client.Authenticator = new AwsAuthenticator(Application_AccessKey, Application_SecretKey, Application_Region, "execute-api");

            //var req = new RestRequest("/solciussb/restapi/v1/applications", Method.POST);
            var req = new RestRequest("/applications", Method.POST);
            req.AddHeader("x-api-key", Application_ApiKey);

            req.JsonSerializer = new RestSharpJsonSerializer();
            req.AddJsonBody(request);

            var resp = client.Execute<ApplicationResponse>(req);

            return resp.Data;
        }
    }
}
