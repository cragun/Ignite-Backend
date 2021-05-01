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
using DataReef.TM.Models.FinancialIntegration.LoanPal;
using System.Threading.Tasks;
using DataReef.TM.Models.DTOs;
using RestSharp.Serializers;
using Newtonsoft.Json.Linq;
using System.Configuration;

namespace DataReef.TM.Services.Services.FinanceAdapters.PropertyNotes
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(AddressFilterMode = AddressFilterMode.Any)]
    [Service(typeof(IPropertyNotesAdapter))]
    public class PropertyNotesAdapter : FinancialAdapterBase, IPropertyNotesAdapter
    {
        private static readonly string url = System.Configuration.ConfigurationManager.AppSettings["PropertyNotes.URL"];
        private static string _senderEmail = ConfigurationManager.AppSettings["SenderEmail"] ?? "donotreply@smartboardcrm.com"; 

        private readonly Lazy<IProposalService> _proposalService;

        public PropertyNotesAdapter(Lazy<IOUSettingService> ouSettingService,
            Lazy<IProposalService> proposalService) : base("PropertyNotes", ouSettingService)
        {
            _proposalService = proposalService;
        }

        private RestClient client
        {
            get
            {
                return new RestClient(url);
            }
        }

        //To create referenceId for Property send from Ignite
        public NoteResponse GetPropertyReferenceId(Property property, string apikey)
        {
            AddPropertyReference req = new AddPropertyReference();
            req.lead_reference = new lead_reference() { ignite_id = Convert.ToString(property.Id), smartboard_id = Convert.ToString(property.SmartBoardId) };
            req.account_reference_id = apikey;

            var propName = property.Name?.FirstAndLastName();

            req.lead_info = new lead_info()
            {
                firstName = propName.Item1,
                lastName = propName.Item2,
                addressLine1 = property.Address1,
                city = property.City,
                state = property.State,
                zipCode = property.ZipCode,
                lattitude = Convert.ToString(property.Latitude),
                longitude = Convert.ToString(property.Longitude),
            };

            var request = new RestRequest($"/references", Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddJsonBody(req);

            var response = client.Execute(request);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                SaveRequest(JsonConvert.SerializeObject(request), response.Content, url, response.StatusCode, null);
                throw new ApplicationException($"GetPropertyReferenceId Failed. {response.ErrorMessage} {response.StatusCode}");
            }
            try
            {
                //SaveRequest(JsonConvert.SerializeObject(request), response.Content, url, response.StatusCode, null);
            }
            catch (Exception)
            {
                SaveRequest(JsonConvert.SerializeObject(request), response.Content, url, response.StatusCode, null);
                throw new ApplicationException($"GetPropertyReferenceId Failed. {response.StatusCode}");
            }

            return JsonConvert.DeserializeObject<NoteResponse>(response.Content);
        }

        //​To get a Properties notes 
        public async Task<List<AllNotes>> GetPropertyNotes(string referenceId)
        {
            var request = new RestRequest($"/notes/{referenceId}", Method.GET);
            var response = await client.ExecuteTaskAsync(request);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                SaveRequest(JsonConvert.SerializeObject(request), response.Content, url, response.StatusCode, null);
                throw new ApplicationException($"GetPropertyNotes Failed. {response.ErrorMessage} {response.StatusCode}");
            }
            try
            {
                SaveRequest(JsonConvert.SerializeObject(request), response.Content, url, response.StatusCode, null);
            }
            catch (Exception)
            {
                throw new ApplicationException($"GetPropertyNotes Failed. {response.StatusCode}");
            }

            if (!String.IsNullOrEmpty(response.Content))
            {
                var res = JsonConvert.DeserializeObject<Dictionary<string, AllNotes>>(response.Content);

                List<AllNotes> notes = new List<AllNotes>();
                foreach (var item in res)
                {
                    notes.Add(item.Value);
                }

                return notes;
            }
            else
            {
                return new List<AllNotes>();
            }
        }

        //​To get a note by id
        public async Task<Models.DTOs.Solar.Finance.Note> GetPropertyNoteById(string noteID)
        {
            var request = new RestRequest($"/notes/getNoteByGuid/{noteID}", Method.GET);
            var response = await client.ExecuteTaskAsync(request);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                SaveRequest(JsonConvert.SerializeObject(request), response.Content, url, response.StatusCode, null);
                throw new ApplicationException($"GetPropertyNoteById Failed. {response.ErrorMessage} {response.StatusCode}");
            }
            try
            {
                SaveRequest(JsonConvert.SerializeObject(request), response.Content, url, response.StatusCode, null);
            }
            catch (Exception)
            {
                throw new ApplicationException($"GetPropertyNoteById Failed. {response.StatusCode}");
            }

            var res = JsonConvert.DeserializeObject<List<Models.DTOs.Solar.Finance.Note>>(response.Content);
            return res?.FirstOrDefault();
        }

        //To create note
        public NoteResponse AddEditNote(string referenceId, PropertyNote note, IEnumerable<Person> taggedPersons, Person user)
        {

            NoteRequest req = new NoteRequest();
            req.message = note.Content;

            string comment = "";
            if (note.ContentType == "Comment")
            {
                req.thread_id = note.ThreadID;
                comment = "/reply";
            }

            RestRequest request = new RestRequest($"/notes{comment}", Method.POST);

            if (!String.IsNullOrEmpty(note.NoteID))
            {
                req.modified = note.DateLastModified?.ToString("MM/dd/yyyy HH:mm:ss");
                request = new RestRequest($"/notes{comment}/{note.NoteID}", Method.PATCH);
            }
            else
            {
                req.guid = Convert.ToString(note.Guid);
                req.referenceId = referenceId;
                req.created = note.DateCreated.ToString("MM/dd/yyyy HH:mm:ss");
                req.source = "Ignite";
                req.attachments = note.Attachments?.Split(',').ToList();
                req.personId = Convert.ToString(note.PersonID);
                req.jobNimbusId = note.JobNimbusID;
                req.jobNimbusLeadId = note.JobNimbusLeadID;
                req.version = note.Version;
                req.propertyType = note.PropertyType;

                if (taggedPersons.Count() > 0)
                {
                    req.taggedUsers = taggedPersons.Select(a => new NoteTaggedUser
                    {
                        email = a.EmailAddressString,
                        phone = a.PhoneNumbers?.FirstOrDefault()?.Number,
                        isSendEmail = false,
                        isSendSms = false,
                        userId = a.SmartBoardID,
                        firstName = a.FirstName,
                        lastName = a.LastName
                    }).Select((s, i) => new { s, i }).ToDictionary(x => x.i, x => x.s);
                }
                else
                {
                    req.taggedUsers = new Dictionary<int, NoteTaggedUser>();
                }

                req.user = new NoteTaggedUser()
                {
                    userId = user.SmartBoardID,
                    email = user.EmailAddressString,
                    firstName = user.FirstName,
                    lastName = user.LastName,
                    phone = user.PhoneNumbers?.FirstOrDefault()?.Number,
                    isSendEmail = false,
                    isSendSms = false,
                };
            }

            request.AddHeader("Content-Type", "application/json");
            request.AddJsonBody(req);

            var response = client.Execute(request);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                SaveRequest(JsonConvert.SerializeObject(request), response.Content, url, response.StatusCode, null);
                //throw new ApplicationException($"AddEditNote Failed. {response.ErrorMessage} {response.StatusCode}");
            }
            try
            {
                //SaveRequest(JsonConvert.SerializeObject(request), response.Content, url, response.StatusCode, null);

                return JsonConvert.DeserializeObject<NoteResponse>(response.Content);
            }
            catch (Exception)
            {
                //throw new ApplicationException($"AddEditNote Failed. {response.StatusCode}");
                SaveRequest(JsonConvert.SerializeObject(request), response.Content, url, response.StatusCode, null);


                return new NoteResponse();
            } 
        }

        //​send email notification
        public async Task<NoteResponse> SendEmailNotification(string subject, string body, string to)
        {
            EmailNotifications req = new EmailNotifications();
            req.type = "email";
            req.subject = subject;
            req.from = _senderEmail;
            req.to = to;
            req.message = body;
  
            var request = new RestRequest($"/notes/sendNotifications", Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddJsonBody(req);

            var response = await client.ExecuteTaskAsync(request);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                SaveRequest(JsonConvert.SerializeObject(request), response.Content, url, response.StatusCode, null);
                throw new ApplicationException($"SendEmailNotification Failed. {response.ErrorMessage} {response.StatusCode}");
            }
            try
            {
                //SaveRequest(JsonConvert.SerializeObject(request), response.Content, url, response.StatusCode, null);
            }
            catch (Exception)
            {
                throw new ApplicationException($"SendEmailNotification Failed. {response.StatusCode}");
            }
             
            return JsonConvert.DeserializeObject<NoteResponse>(response.Content);
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
