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
using System.Web.Http;
using System.Net.Http;

namespace DataReef.TM.Services.Services.FinanceAdapters.JobNimbus
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [Service(typeof(IJobNimbusAdapter))]
    public class JobNimbusAdapter : FinancialAdapterBase, IJobNimbusAdapter
    {
        public JobNimbusAdapter(Lazy<IOUSettingService> ouSettingService) : base("JobNimbus", ouSettingService)
        {
        }

        public JobNimbusLeadResponseData CreateJobNimbusLead(Guid propertyid, string url, string apikey)
        {
            using (var dc = new DataContext())
            {
                var property = dc.Properties.Include("PropertyBag").Where(x => x.Guid == propertyid).FirstOrDefault();

                if (property == null)
                {
                    throw new HttpResponseException(new HttpResponseMessage() { StatusCode = HttpStatusCode.NotFound, ReasonPhrase = "The property was not found for Jobnimbus." });
                }
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

                url = $"{url}/api1/contacts/";
                if (!string.IsNullOrEmpty(property.JobNimbusLeadID))
                    url = $"{url}{property.JobNimbusLeadID}";

                var request = new RestRequest(url, Method.POST);
                request.AddHeader("Authorization", $"Bearer {apikey}");
                request.AddHeader("Content-Type", "application/json");
                request.AddParameter("application/json", new JavaScriptSerializer().Serialize(req), ParameterType.RequestBody);

                RestClient client = new RestClient(url);
                var response = client.Execute(request);
                // var response = await client.ExecuteTaskAsync(request);

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    SaveRequest(JsonConvert.SerializeObject(request), response.Content, url, response.StatusCode, apikey);
                    throw new HttpResponseException(new HttpResponseMessage() { StatusCode = HttpStatusCode.NotFound, ReasonPhrase = $"CreateJobNimbusLead Failed. {response.Content}  {response.StatusCode}" });
                }
                try
                {
                    SaveRequest(JsonConvert.SerializeObject(request), response.Content, url, response.StatusCode, apikey);
                }
                catch (Exception)
                {
                    throw new HttpResponseException(new HttpResponseMessage() { StatusCode = HttpStatusCode.NotFound, ReasonPhrase = $"CreateJobNimbusLead Failed.   {response.StatusCode}" });
                }

                var lead = JsonConvert.DeserializeObject<JobNimbusLeadResponseData>(response.Content);

                property.JobNimbusLeadID = lead != null ? lead.jnid : "";
                dc.SaveChanges();

                return lead;
            }
        }

        public AppointmentJobNimbusLeadResponseData CreateAppointmentJobNimbusLead(Guid appid, string url, string apikey)
        {
            using (var dc = new DataContext())
            {
                var appointment = dc.Appointments.Where(x => x.Guid == appid).FirstOrDefault();

                AppointmentJobNimbusLeadRequestData req = new AppointmentJobNimbusLeadRequestData();

                List<related> relate = new List<related>();

                relate.Add(new related { id = appointment.JobNimbusLeadID });
                req.related = relate;
                req.record_type_name = "Appointment";
                req.title = appointment.Details;
                req.date_start = (long)(appointment.StartDate - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
                req.created_by = Convert.ToString(appointment?.CreatedByID);
                req.customer = Convert.ToString(appointment?.CreatedByID);

                url = $"{url}/api1/tasks/";
                if (!string.IsNullOrEmpty(appointment.JobNimbusLeadID))
                    url = $"{url}{appointment.JobNimbusID}";

                var request = new RestRequest(url, Method.POST);
                request.AddHeader("Authorization", $"Bearer {apikey}");
                request.AddHeader("Content-Type", "application/json");
                request.AddParameter("application/json", new JavaScriptSerializer().Serialize(req), ParameterType.RequestBody);

                RestClient client = new RestClient(url);
                var response = client.Execute(request);

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    SaveRequest(JsonConvert.SerializeObject(request), response.Content, url, response.StatusCode, apikey);
                    throw new ApplicationException($"CreateJobNimbusAppointment Failed. {response.ErrorMessage}  {response.StatusCode}");
                }
                try
                {
                    SaveRequest(JsonConvert.SerializeObject(request), response.Content, url, response.StatusCode, apikey);
                }
                catch (Exception)
                {
                    throw new ApplicationException($"CreateJobNimbusAppointment Failed. {response.StatusCode}");
                }

                var ret = JsonConvert.DeserializeObject<AppointmentJobNimbusLeadResponseData>(response.Content);

                appointment.JobNimbusID = ret != null ? ret.jnid : "";
                dc.SaveChanges();
                return ret;
            }
        }

        public NoteJobNimbusLeadResponseData CreateJobNimbusNote(PropertyNote note, string url, string apikey)
        {
            using (var dc = new DataContext())
            {
                NoteJobNimbusLeadRequestData req = new NoteJobNimbusLeadRequestData();
                req.primary = new primary() { id = note.JobNimbusID };
                req.record_type_name = "Note";
                req.note = note.Content;
                req.date_created = (long)(note.DateCreated - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
                req.created_by = Convert.ToString(note?.PersonID);
                req.external_id = Convert.ToString(note?.Guid);

                url = $"{url}/api1/activities/";
                var request = new RestRequest(url, Method.POST);
                request.AddHeader("Authorization", $"Bearer {apikey}");
                request.AddHeader("Content-Type", "application/json");
                request.AddParameter("application/json", new JavaScriptSerializer().Serialize(req), ParameterType.RequestBody);

                RestClient client = new RestClient(url);
                var response = client.Execute(request);

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    SaveRequest(JsonConvert.SerializeObject(request), response.Content, url, response.StatusCode, apikey);
                    throw new ApplicationException($"CreateJobNimbusLead Failed. {response.ErrorMessage}  {response.StatusCode}");
                }
                try
                {
                    SaveRequest(JsonConvert.SerializeObject(request), response.Content, url, response.StatusCode, apikey);
                }
                catch (Exception)
                {
                    throw new ApplicationException($"CreateJobNimbusNote Failed.  {response.StatusCode}");
                }

                return JsonConvert.DeserializeObject<NoteJobNimbusLeadResponseData>(response.Content);
            }
        }


        public List<JobNimbusContacts> GetJobNimbusContacts(string JobNimbusLeadID, string url, string apikey)
        {
            using (var dc = new DataContext())
            {
                var request = new RestRequest($"/api1/contacts", Method.GET);
                request.AddHeader("Authorization", $"Bearer {apikey}");

                RestClient client = new RestClient(url);
                var response = client.Execute(request);

                //var response = await client.ExecuteTaskAsync(request);

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new ApplicationException($"GetJobNimbusContacts Failed. {response.Content}  {response.StatusCode}");
                }

                try
                {
                    SaveRequest(JsonConvert.SerializeObject(request), response, url + "/api1/contacts", null, apikey);
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
