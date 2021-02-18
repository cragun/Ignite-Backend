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
using DataReef.Core.Infrastructure.Authorization;

namespace DataReef.TM.Services.Services.FinanceAdapters.JobNimbus
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    //[ServiceBehavior(AddressFilterMode = AddressFilterMode.Any)]
    [Service(typeof(IJobNimbusAdapter))]
    public class JobNimbusAdapter : FinancialAdapterBase, IJobNimbusAdapter
    {
        private static readonly string url = System.Configuration.ConfigurationManager.AppSettings["JobNimbus.url"];
        private static readonly string AuthUsername = System.Configuration.ConfigurationManager.AppSettings["JobNimbus.Auth.Username"];
        private static readonly string AuthPassword = System.Configuration.ConfigurationManager.AppSettings["JobNimbus.Auth.Password"];
        private static readonly string AuthTokenApikey = System.Configuration.ConfigurationManager.AppSettings["JobNimbus.Auth.TokenApikey"];
        private string id;

        //private static readonly string url = "https://app.jobnimbus.com";
        //private static readonly string AuthUsername = "hevin.android@gmail.com";
        //private static readonly string AuthPassword = "Hevin@123";
        //private static readonly string AuthTokenApikey = "kkmlkldvbrphau9t";

        public JobNimbusAdapter(Lazy<IOUSettingService> ouSettingService) : base("JobNimbus", ouSettingService)
        {
        }

        private RestClient client
        {
            get
            {
                return new RestClient(url);
            }
        }  

        public JobNimbusLeadResponseData CreateJobNimbusLead(Property property)
        {
            using (var dc = new DataContext())
            {

                Geo geo = new Geo();
                geo.lat = Convert.ToDouble(String.Format("{0:0.0000}", property.Latitude));
                geo.lon = Convert.ToDouble(String.Format("{0:0.0000}", property.Longitude));

                var name = property.GetMainOccupant();

                JobNimbusLeadRequestData req = new JobNimbusLeadRequestData();
                req.first_name = name?.FirstName == null ? "" : name?.FirstName;
                req.last_name = name?.LastName == null ? "" : name?.LastName;
                req.display_name = $"{req.first_name} {req.last_name}";
                req.record_type_name = "Customer";
                req.status_name = "Lead";
                req.geo = geo;

                var request = new RestRequest($"/api1/contacts", Method.POST);
                request.AddHeader("Authorization", $"Bearer {AuthTokenApikey}");
                request.AddHeader("Content-Type", "application/json");
                request.AddParameter("application/json", new JavaScriptSerializer().Serialize(req), ParameterType.RequestBody);

                SaveRequest(JsonConvert.SerializeObject(request), "response", url + "/api1/contacts", null, AuthTokenApikey);

                var response = client.Execute(request);

                if (response.StatusCode != HttpStatusCode.Created)
                {
                    SaveRequest(JsonConvert.SerializeObject(request), response.Content, url + "/api1/contacts", null, AuthTokenApikey);
                    throw new ApplicationException($"CreateJobNimbusLead Failed. {response.Content}");
                }
                try
                {
                    SaveRequest(JsonConvert.SerializeObject(request), response.Content, url + "/api1/contacts", null, AuthTokenApikey);
                }
                catch (Exception)
                {
                    throw new ApplicationException($"CreateJobNimbusLead Failed.");
                }

                var ret = JsonConvert.DeserializeObject<JobNimbusLeadResponseData>(response.Content);
                return ret;
            }
        }

        public AppointmentJobNimbusLeadResponseData CreateAppointmentJobNimbusLead(Appointment appointment)
        {
            using (var dc = new DataContext())
            {
                AppointmentJobNimbusLeadRequestData req = new AppointmentJobNimbusLeadRequestData();

                var prop = dc.Properties.FirstOrDefault(p => p.Guid == appointment.PropertyID);

                List<related> relate = new List<related>();
                related rlat = new related();
                rlat.id = prop.JobNimbusLeadID;

                relate.Add(rlat);
                req.related = relate;
                req.record_type_name = "Appointment";
                req.title = appointment.Details;
                req.date_start = appointment.StartDate;
                req.date_end = appointment.EndDate;

                var request = new RestRequest($"/api1/tasks", Method.POST);
                request.AddJsonBody(req);
                request.AddHeader("Authorization", "Bearer " + AuthTokenApikey);

                var json = new JavaScriptSerializer().Serialize(req);
                SaveRequest(JsonConvert.SerializeObject(request), "response", url + "/api1/tasks", null, AuthTokenApikey);

                var response = client.Execute(request);
                var resp = new JavaScriptSerializer().Serialize(response);
                SaveRequest(JsonConvert.SerializeObject(request), resp, url + "/api1/tasks", null, AuthTokenApikey);

                if (response.StatusCode != HttpStatusCode.Created)
                {
                    SaveRequest(JsonConvert.SerializeObject(request), resp, url + "/api1/tasks", null, AuthTokenApikey);
                    throw new ApplicationException($"CreateJobNimbusLead Failed. {response.Content}");
                }

                try
                {
                    SaveRequest(JsonConvert.SerializeObject(request), resp, url + "/api1/tasks", null, AuthTokenApikey);
                }
                catch (Exception)
                {
                    throw new ApplicationException($"CreateJobNimbusLead Failed.");
                }


                var content = response.Content;
                var ret = JsonConvert.DeserializeObject<AppointmentJobNimbusLeadResponseData>(content);
                var JobNimbusLeadID = ret != null ? ret.jnid : "";

                return ret;
            }
        }

        public List<JobNimbusContacts> GetJobNimbusContacts(string JobNimbusLeadID)
        {
            using (var dc = new DataContext())
            {
                //https://app.jobnimbus.com/api1/contacts
                var request = new RestRequest($"/api1/contacts", Method.GET);
                request.AddHeader("Authorization", "Bearer " + AuthTokenApikey);

                var response = client.Execute(request);

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new ApplicationException($"GetJobNimbusContacts Failed. {response.Content}");
                }

                try
                {
                    SaveRequest(JsonConvert.SerializeObject(request), response, url + "/api1/contacts", null, AuthTokenApikey);
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
