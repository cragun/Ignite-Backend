using DataReef.Core.Attributes;
using DataReef.TM.Contracts.Services;
using DataReef.TM.Contracts.Services.FinanceAdapters;
using DataReef.TM.Models.DTOs.Solar.Finance;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
        private static readonly string FrameUrl = System.Configuration.ConfigurationManager.AppSettings["Sunlight.Frame.Url"];

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

        public class TokenResponse
        {
            public string access_token { get; set; }
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

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new ApplicationException($"GetSunlightToken Failed. {response.Content}");
                }

                var content = response.Content;
                var ret = JsonConvert.DeserializeObject<TokenResponse>(content);

                return ret.access_token;


            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public class SunlightProjects
        {
            public List<Projects> Projects { get; set; }
        }

        public string CreateSunlightApplicant(string fname, string lname, string email, string phone, string street, string city, string state, string zipcode)
        {
            try
            {
                SunlightProjects req = new SunlightProjects();
                Projects project = new Projects();
                Applicants applicnt = new Applicants();

                applicnt.firstName = fname;
                applicnt.lastName = lname;
                applicnt.email = email;
                applicnt.phone = phone;
                applicnt.isPrimary = true;

                project.applicants = new List<Applicants>();
                project.applicants.Add(applicnt);
                project.installStreet = street;
                project.installCity = city;
                project.installStateName = state;
                project.installZipCode = zipcode;

                req.Projects = new List<Projects>();
                req.Projects.Add(project);                

                string token = GetSunlightToken();
                string svcCredentials = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(AuthUsername + ":" + AuthPassword));
                var request = new RestRequest($"/applicant/create/", Method.POST);
                request.AddJsonBody(req);
                request.AddHeader("Authorization", "Basic " + svcCredentials);
                request.AddHeader("SFAccessToken", "Bearer " + token);
                
                var response = client.Execute(request);

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new ApplicationException($"CreateSunlightApplicant Failed. {response.Content}");
                }

                var content = response.Content;
                var ret = JsonConvert.DeserializeObject<SunlightProjects>(content);
                string frame = FrameUrl.Replace("{tokenid}", token).Replace("{hashid}", "&pid=" + ret.Projects?.FirstOrDefault().hashId);

                return frame;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

        }
    }
}
