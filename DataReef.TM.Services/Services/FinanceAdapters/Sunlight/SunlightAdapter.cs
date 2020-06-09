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


        public string GetfullState(string shortState)
        {
            var list = new List<KeyValuePair<string, string>>();
            list.Add(new KeyValuePair<string, string>("AL", "Alabama"));
            list.Add(new KeyValuePair<string, string>("AK", "Alaska"));
            list.Add(new KeyValuePair<string, string>("AZ", "Arizona"));
            list.Add(new KeyValuePair<string, string>("AR", "Arkansas"));
            list.Add(new KeyValuePair<string, string>("CA", "California"));
            list.Add(new KeyValuePair<string, string>("CO", "Colorado"));
            list.Add(new KeyValuePair<string, string>("CT", "Connecticut"));
            list.Add(new KeyValuePair<string, string>("DE", "Delaware"));
            list.Add(new KeyValuePair<string, string>("FL", "Florida"));
            list.Add(new KeyValuePair<string, string>("GA", "Georgia"));
            list.Add(new KeyValuePair<string, string>("HI", "Hawaii"));
            list.Add(new KeyValuePair<string, string>("ID", "Idaho"));
            list.Add(new KeyValuePair<string, string>("IL", "Illinois"));
            list.Add(new KeyValuePair<string, string>("IN", "Indiana"));
            list.Add(new KeyValuePair<string, string>("IA", "Iowa"));
            list.Add(new KeyValuePair<string, string>("KS", "Kansas"));
            list.Add(new KeyValuePair<string, string>("KY", "Kentucky"));
            list.Add(new KeyValuePair<string, string>("LA", "Louisiana"));
            list.Add(new KeyValuePair<string, string>("ME", "Maine"));
            list.Add(new KeyValuePair<string, string>("MD", "Maryland"));
            list.Add(new KeyValuePair<string, string>("MA", "Massachusetts"));
            list.Add(new KeyValuePair<string, string>("MI", "Michigan"));
            list.Add(new KeyValuePair<string, string>("MN", "Minnesota"));
            list.Add(new KeyValuePair<string, string>("MS", "Mississippi"));
            list.Add(new KeyValuePair<string, string>("MO", "Missouri"));
            list.Add(new KeyValuePair<string, string>("MT", "Montana"));
            list.Add(new KeyValuePair<string, string>("NE", "Nebraska"));
            list.Add(new KeyValuePair<string, string>("NV", "Nevada"));
            list.Add(new KeyValuePair<string, string>("NH", "New Hampshire"));
            list.Add(new KeyValuePair<string, string>("NJ", "New Jersey"));
            list.Add(new KeyValuePair<string, string>("NM", "New Mexico"));
            list.Add(new KeyValuePair<string, string>("NY", "New York"));
            list.Add(new KeyValuePair<string, string>("NC", "North Carolina"));
            list.Add(new KeyValuePair<string, string>("ND", "North Dakota"));
            list.Add(new KeyValuePair<string, string>("OH", "Ohio"));
            list.Add(new KeyValuePair<string, string>("OK", "Oklahoma"));
            list.Add(new KeyValuePair<string, string>("OR", "Oregon"));
            list.Add(new KeyValuePair<string, string>("PA", "Pennsylvania"));
            list.Add(new KeyValuePair<string, string>("RI", "Rhode Island"));
            list.Add(new KeyValuePair<string, string>("SC", "South Carolina"));
            list.Add(new KeyValuePair<string, string>("SD", "South Dakota"));
            list.Add(new KeyValuePair<string, string>("TN", "Tennessee"));
            list.Add(new KeyValuePair<string, string>("TX", "Texas"));
            list.Add(new KeyValuePair<string, string>("UT", "Utah"));
            list.Add(new KeyValuePair<string, string>("VT", "Vermont"));
            list.Add(new KeyValuePair<string, string>("VA", "Virginia"));
            list.Add(new KeyValuePair<string, string>("WA", "Washington"));
            list.Add(new KeyValuePair<string, string>("WV", "West Virginia"));
            list.Add(new KeyValuePair<string, string>("WI", "Wisconsin"));
            list.Add(new KeyValuePair<string, string>("WY", "Wyoming"));


            string fullState = list.Where(x => x.Key.ToLower() == shortState.ToLower()).FirstOrDefault().Value;

            return fullState;
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
            public string returnCode { get; set; }
            public List<Projects> Projects { get; set; }
        }

        public string CreateSunlightApplicant(string fname, string lname, string email, string phone, string street, string city, string state, string zipcode)
        {
            try
            {

                state = GetfullState(state);

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


        public string CreateSunlightAccount(string fname, string lname, string email, string phone, string street, string city, string state, string zipcode)
        {
            try
            {

                state = GetfullState(state);

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
