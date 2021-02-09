using DataReef.Core.Attributes;
using DataReef.TM.Contracts.Services.FinanceAdapters;
using DataReef.TM.DataAccess.Database;
using DataReef.TM.Models;
using DataReef.TM.Models.DTOs.Solar.Finance;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Text;
using System.Web.Script.Serialization;
using DataReef.TM.Services;
using DataReef.TM.Contracts.Services;
using System.Linq;

namespace DataReef.TM.Services.Services.FinanceAdapters.JobNimbus
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    //[ServiceBehavior(AddressFilterMode = AddressFilterMode.Any)]
    [Service(typeof(IJobNimbusAdapter))]
    public class JobNimbusAdapter : FinancialAdapterBase, IJobNimbusAdapter
    {
        //private static readonly string url = System.Configuration.ConfigurationManager.AppSettings["JobNimbus.test.url"];
        private static readonly string url = System.Configuration.ConfigurationManager.AppSettings["JobNimbus.url"];
        private static readonly string AuthUsername = System.Configuration.ConfigurationManager.AppSettings["JobNimbus.Auth.Username"];
        private static readonly string AuthPassword = System.Configuration.ConfigurationManager.AppSettings["JobNimbus.Auth.Password"];
        //private static readonly string url = "https://app.jobnimbus.com";
        //private static readonly string AuthUsername = "hevin.android@gmail.com";
        //private static readonly string AuthPassword = "Hevin@123";

        public JobNimbusAdapter(Lazy<IOUSettingService> ouSettingService) : base("JobNimbus", ouSettingService)
        {
        }

        public class TokenResponse
        {
            public string token { get; set; }
        }

        private RestClient client
        {
            get
            {
                return new RestClient(url);
            }
        }

        public class JobNimbusLeadData
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string display_name { get; set; }
            public string Email { get; set; }
            public string Preferred_Contact_Method { get; set; }
            public string Preferred_Language { get; set; }
            public Phonenum Phone { get; set; }
            public Addresses Address { get; set; }
            public string status_name { get; set; }
        }

        public string GetJobNimbusToken()
        {
            try
            {
                string svcCredentials = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(AuthUsername + ":" + AuthPassword));

                var request = new RestRequest($"/authentication", Method.GET);
                request.AddHeader("Authorization", "Basic " + svcCredentials);
                var response = client.Execute(request);

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new ApplicationException($"GetJobNimbusToken Failed. {response.Content}");
                }

                var content = response.Content;
                var ret = JsonConvert.DeserializeObject<TokenResponse>(content);

                return ret.token;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public List<JobNimbusLead> CreateJobNimbusLead(Property property)
        {
            using (var dc = new DataContext())
            {
                JobNimbusLeadData req = new JobNimbusLeadData();

                string number = property.GetMainPhoneNumber()?.Replace("-", "");
                string email = property.GetMainEmailAddress();

                Phonenum phone = new Phonenum();
                phone.Number = number == null ? "" : number;
                phone.Type = "Mobile";

                Addresses addr = new Addresses();
                addr.City = property.City;
                addr.Country = "USA";
                addr.Latitude = Convert.ToDouble(String.Format("{0:0.0000}", property.Latitude));
                addr.Longitude = Convert.ToDouble(String.Format("{0:0.0000}", property.Longitude));
                addr.PostalCode = property.ZipCode;
                addr.State = property.State == null ? "" : property.State;
                addr.Street = property.Address1 == null ? "" : property.Address1;
                                                                                                                       
                var name = property.GetMainOccupant();

                req.FirstName = name?.FirstName == null ? "" : name?.FirstName;
                req.LastName = name?.LastName == null ? "" : name?.LastName;
                req.display_name = name?.MiddleInitial == null ? "" : name?.MiddleInitial;
                req.Email = email == null ? "" : email;
                req.Phone = phone;
                req.Address = addr;
                req.Preferred_Contact_Method = "Email";
                req.Preferred_Language = "English";
                req.status_name = "Lead";

                string token = GetJobNimbusToken();
                var request = new RestRequest($"/api1/contacts", Method.POST);
                request.AddJsonBody(req);
                request.AddHeader("Authorization", "Bearer " + token);

                var response = client.Execute(request);
                var json = new JavaScriptSerializer().Serialize(req);
                var resp = new JavaScriptSerializer().Serialize(response);


                if (response.StatusCode != HttpStatusCode.Created)
                {
                    SaveRequest(JsonConvert.SerializeObject(request), response, url + "/api1/contacts", null, token);
                    throw new ApplicationException($"CreateJobNimbusLead Failed. {response.Content}");
                }

                try
                {
                    SaveRequest(JsonConvert.SerializeObject(request), response, url + "/api1/contacts", null, token);
                }
                catch (Exception)
                {
                }

                var content = response.Content;
                var ret = JsonConvert.DeserializeObject<List<JobNimbusLead>>(content);
                var JobNimbusLeadID = ret.FirstOrDefault() != null ? ret.FirstOrDefault().lead.Id : "";

                GetJobNimbusContacts(JobNimbusLeadID);

                return ret;
            }
        }

        public List<JobNimbusContacts> GetJobNimbusContacts(string JobNimbusLeadID)
        {
            using (var dc = new DataContext())
            {
                //https://app.jobnimbus.com/api1/contacts
                string token = GetJobNimbusToken();
                var request = new RestRequest($"/api1/contacts", Method.GET);
                request.AddHeader("Authorization", "Bearer " + token);

                var response = client.Execute(request);

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new ApplicationException($"GetJobNimbusContacts Failed. {response.Content}");
                }

                try
                {
                    SaveRequest(JsonConvert.SerializeObject(request), response, url + "/api1/contacts", null, token);
                }
                catch (Exception)
                {
                }

                var content = response.Content;
                var ret = JsonConvert.DeserializeObject<List<JobNimbusContacts>>(content);

                return ret;
            }

        }

        public override string GetBaseUrl(Guid ouid)
        {
            throw new NotImplementedException();
        }

        public override AuthenticationContext GetAuthenticationContext(Guid ouid)
        {
            throw new NotImplementedException();
        }

        public override Services.TokenResponse AuthorizeAdapter(AuthenticationContext authenticationContext)
        {
            throw new NotImplementedException();
        }

        public override Dictionary<string, string> GetCustomHeaders(Services.TokenResponse tokenResponse)
        {
            throw new NotImplementedException();
        }
    }
}
