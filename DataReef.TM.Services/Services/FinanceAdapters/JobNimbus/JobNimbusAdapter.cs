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
using System.Threading.Tasks;

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

        public JobNimbusLeadResponseData CreateJobNimbusLead(Property property, bool IsCreate)
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
                req.display_name = property.Name;
                req.address_line1 = property.Address1;
                req.address_line2 = property.Address2;
                req.city = property.City;
                req.state_text = property.State;
                req.zip = property.ZipCode;
                req.customer = Convert.ToString(property.Guid);
                req.record_type_name = "Customer";
                req.status_name = "Lead";
                req.geo = geo;

                string url = $"/api1/contacts/";
                if (!IsCreate)
                    url = $"{url}{property.JobNimbusLeadID}";

                var request = new RestRequest(url, Method.POST);
                request.AddHeader("Authorization", $"Bearer {AuthTokenApikey}");
                request.AddHeader("Content-Type", "application/json");
                request.AddParameter("application/json", new JavaScriptSerializer().Serialize(req), ParameterType.RequestBody);

                var response = client.Execute(request);

                if (response.StatusCode != HttpStatusCode.Created)
                {
                    SaveRequest(JsonConvert.SerializeObject(request), response.Content, url, null, AuthTokenApikey);
                    throw new ApplicationException($"CreateJobNimbusLead Failed. {response.Content}");
                }
                try
                {
                    SaveRequest(JsonConvert.SerializeObject(request), response.Content, url, null, AuthTokenApikey);
                }
                catch (Exception)
                {
                    throw new ApplicationException($"CreateJobNimbusLead Failed.");
                }

                return JsonConvert.DeserializeObject<JobNimbusLeadResponseData>(response.Content);
            }
        }

        public AppointmentJobNimbusLeadResponseData CreateAppointmentJobNimbusLead(Appointment appointment, bool IsCreate)
        {
            using (var dc = new DataContext())
            {
                AppointmentJobNimbusLeadRequestData req = new AppointmentJobNimbusLeadRequestData();

                List<related> relate = new List<related>(); 

                relate.Add(new related { id = appointment.JobNimbusLeadID });
                req.related = relate;
                req.record_type_name = "Appointment";
                req.title = appointment.Details;
                req.date_start = (long)(appointment.StartDate - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
                req.created_by = Convert.ToString(appointment?.CreatedByID);
                req.customer = Convert.ToString(appointment?.CreatedByID);

                string url = $"/api1/tasks/";
                if (!IsCreate)
                    url = $"{url}{appointment.JobNimbusID}";

                var request = new RestRequest(url, Method.POST);
                request.AddHeader("Authorization", $"Bearer {AuthTokenApikey}");
                request.AddHeader("Content-Type", "application/json");
                request.AddParameter("application/json", new JavaScriptSerializer().Serialize(req), ParameterType.RequestBody);

                var response = client.Execute(request);

                if (response.StatusCode != HttpStatusCode.Created)
                {
                    SaveRequest(JsonConvert.SerializeObject(request), response.Content, url , null, AuthTokenApikey);
                    throw new ApplicationException($"CreateJobNimbusAppointment Failed. {response.ErrorMessage}");
                }
                try
                {
                    SaveRequest(JsonConvert.SerializeObject(request), response.Content, url , null, AuthTokenApikey);
                }
                catch (Exception)
                {
                    throw new ApplicationException($"CreateJobNimbusAppointment Failed.");
                }

                var ret = JsonConvert.DeserializeObject<AppointmentJobNimbusLeadResponseData>(response.Content);
                return ret;
            }
        }

        public async Task<NoteJobNimbusLeadResponseData> CreateJobNimbusNote(PropertyNote note)
        {
            using (var dc = new DataContext())
            {
                NoteJobNimbusLeadRequestData req = new NoteJobNimbusLeadRequestData();
                req.primary = new List<primary>() { new primary() { id = note.JobNimbusID } };
                req.record_type_name = "Note";
                req.note = note.Content;
                req.date_created = (long)(note.DateCreated - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
                req.created_by = Convert.ToString(note?.PersonID);
                req.external_id = Convert.ToString(note?.Guid);

                string url = $"/api1/activities";
                var request = new RestRequest(url, Method.POST);
                request.AddHeader("Authorization", $"Bearer {AuthTokenApikey}");
                request.AddHeader("Content-Type", "application/json");
                request.AddParameter("application/json", new JavaScriptSerializer().Serialize(req), ParameterType.RequestBody);

                var response = client.Execute(request);

                if (response.StatusCode != HttpStatusCode.Created)
                {
                    SaveRequest(JsonConvert.SerializeObject(request), response.Content, url, null, AuthTokenApikey);
                    throw new ApplicationException($"CreateJobNimbusLead Failed. {response.ErrorMessage}");
                }
                try
                {
                    SaveRequest(JsonConvert.SerializeObject(request), response.Content, url, null, AuthTokenApikey);
                }
                catch (Exception)
                {
                    throw new ApplicationException($"CreateJobNimbusNote Failed.");
                }

                return JsonConvert.DeserializeObject<NoteJobNimbusLeadResponseData>(response.Content);
            }
        }

        public List<JobNimbusContacts> GetJobNimbusContacts(string JobNimbusLeadID)
        {
            using (var dc = new DataContext())
            {
                var request = new RestRequest($"/api1/contacts", Method.GET);
                request.AddHeader("Authorization", $"Bearer {AuthTokenApikey}");

                var response = client.Execute(request);

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new ApplicationException($"GetJobNimbusContacts Failed. {response.Content}");
                }

                try
                {
                    SaveRequest(JsonConvert.SerializeObject(request), response, url + "/api1/contacts", null, AuthTokenApikey);
                }
                catch (Exception ex)
                {
                }

                return JsonConvert.DeserializeObject<List<JobNimbusContacts>>(response.Content);
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
