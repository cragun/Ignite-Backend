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

namespace DataReef.TM.Services.Services.FinanceAdapters.Sunnova
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(AddressFilterMode = AddressFilterMode.Any)]
    [Service(typeof(ISunnovaAdapter))]
    public class SunnovaAdapter : ISunnovaAdapter
    {
        private static readonly string url = System.Configuration.ConfigurationManager.AppSettings["Sunnova.test.url"];
        private static readonly string AuthUsername = System.Configuration.ConfigurationManager.AppSettings["Sunnova.Auth.Username"];
        private static readonly string AuthPassword = System.Configuration.ConfigurationManager.AppSettings["Sunnova.Auth.Password"];

        private RestClient client
        {
            get
            {
                return new RestClient(url);
            }
        }

        public class TokenResponse
        {
            public string token { get; set; }
        }


        // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);        

        public class SunnovaProjects
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Middle_Name { get; set; }
            public string Suffix { get; set; }
            public string Email { get; set; }
            public string Preferred_Contact_Method { get; set; }
            public string Preferred_Language { get; set; }
            public Phone Phone { get; set; }
            public Addresss Address { get; set; }
            public string external_id { get; set; }
        }

        public string GetSunnovaToken()
        {
            try
            {
                string svcCredentials = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(AuthUsername + ":" + AuthPassword));

                var request = new RestRequest($"/gettoken/authentication", Method.GET);
                request.AddHeader("Authorization", "Basic " + svcCredentials);
                var response = client.Execute(request);               

                var json = new JavaScriptSerializer().Serialize(response);

                ApiLogEntry apilog = new ApiLogEntry();
                apilog.Id = Guid.NewGuid();
                apilog.User = "testuser";
                apilog.Machine = Environment.MachineName;
                apilog.RequestContentType = "";
                apilog.RequestRouteTemplate = "";
                apilog.RequestRouteData = "";
                apilog.RequestIpAddress = "";
                apilog.RequestMethod = "sunnovatokenresponse";
                apilog.RequestHeaders = "";
                apilog.RequestTimestamp = DateTime.UtcNow;
                apilog.RequestUri = "";
                apilog.ResponseContentBody = "";
                apilog.RequestContentBody = json.ToString();

                using (var db = new DataContext())
                {
                    db.ApiLogEntries.Add(apilog);
                    db.SaveChanges();
                }

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new ApplicationException($"GetSunnovaToken Failed. {response.Content}");
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


        public List<SunnovaLead> CreateSunnovaLead(Property property)
        {
            using (var dc = new DataContext())
            {
                SunnovaProjects req = new SunnovaProjects();
                
                string number = property.GetMainPhoneNumber()?.Replace("-", "");
                string email = property.GetMainEmailAddress();

                Phone phone = new Phone();
                phone.Number = number == null ? "" : number;                                                                                                       
                phone.Type = "Mobile";

                Addresss addr = new Addresss();
                addr.City = property.City;
                addr.Country = "USA"; 
                addr.Latitude = Convert.ToDouble(String.Format("{0:0.0000}", property.Latitude)); 
                addr.Longitude = Convert.ToDouble(String.Format("{0:0.0000}", property.Longitude));
                addr.PostalCode = property.ZipCode;
                addr.State = property.State;
                addr.Street = property.Address1;

                var name = property.GetMainOccupant();

                req.FirstName = name?.FirstName == null ? "" : name?.FirstName;
                req.LastName = name?.LastName == null ? "" : name?.LastName;
                req.Middle_Name = name?.MiddleInitial == null ? "" : name?.MiddleInitial;
                req.Email = email == null ? "" : email;
                req.Phone = phone;
                req.Address = addr;
                req.external_id = property.ExternalID;
                req.Preferred_Contact_Method = "Email";
                req.Preferred_Language = "English";
                req.Suffix = "";

                string token = GetSunnovaToken();
            // https://apitest.sunnova.com/services/v1.0/leads
                var request = new RestRequest($"/services/v1.0/leads", Method.POST);
                request.AddJsonBody(req);
                request.AddHeader("Authorization", "Bearer " + token);

                var json = new JavaScriptSerializer().Serialize(req);

                ApiLogEntry apilog = new ApiLogEntry();
                apilog.Id = Guid.NewGuid();
                apilog.User = "testuser";
                apilog.Machine = Environment.MachineName;
                apilog.RequestContentType = token;
                apilog.RequestRouteTemplate = "";
                apilog.RequestRouteData = "";
                apilog.RequestIpAddress = "";
                apilog.RequestMethod = "sunnovaleadresquest";
                apilog.RequestHeaders = "";
                apilog.RequestTimestamp = DateTime.UtcNow;
                apilog.RequestUri = "";
                apilog.ResponseContentBody = "";
                apilog.RequestContentBody = json.ToString();

                using (var db = new DataContext())
                {
                    db.ApiLogEntries.Add(apilog); 
                    db.SaveChanges();
                }
                 
                var response = client.Execute(request);

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new ApplicationException($"CreateSunnovaLead Failed. {response.Content}");
                }

                var content = response.Content;
                var ret = JsonConvert.DeserializeObject<List<SunnovaLead>>(content);

                return ret;
            }
        }

    }
}
