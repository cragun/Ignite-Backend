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

namespace DataReef.TM.Services.Services.FinanceAdapters.PropertyNotes
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(AddressFilterMode = AddressFilterMode.Any)]
    [Service(typeof(IPropertyNotesAdapter))]
    public class PropertyNotesAdapter : FinancialAdapterBase, IPropertyNotesAdapter
    {
        private static readonly string url = System.Configuration.ConfigurationManager.AppSettings["PropertyNotes.URL"];

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
                SaveRequest(JsonConvert.SerializeObject(request), response.Content, url, response.StatusCode, null);
            }
            catch (Exception)
            {
                throw new ApplicationException($"GetPropertyReferenceId Failed. {response.StatusCode}");
            }

            var data = JObject.Parse(response.Content);
            return JsonConvert.DeserializeObject<NoteResponse>(response.Content);
        }

        //​To get a Properties note
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

            //RestResponse response = new RestResponse();
            //response.Content = "{\"f6bd7533f3209b9fe6893e942e573297ecba453e\":{\"notes\":{\"attachments\":null,\"taggedUsers\":[{\"Key\":0,\"Value\":{\"email\":\"mdhakecha+1@gmail.com\",\"firstName\":\"Manish\",\"lastName\":\"Dhakecha\",\"phone\":null,\"isSendEmail\":true,\"isSendSms\":true,\"userId\":\"18\"}}],\"parentIds\":[\"606d86d0998f280007017ac3\"],\"user\":[{\"email\":\"mdhakecha+1@gmail.com\",\"firstName\":\"Manish\",\"lastName\":\"Dhakecha\",\"phone\":null,\"isSendEmail\":true,\"isSendSms\":true,\"userId\":\"18\"}],\"pinnedBy\":[],\"likedBy\":[],\"_id\":\"607d878e19122600071aa2b3\",\"referenceId\":\"606d86d0998f280007017ac3\",\"threadId\":\"f6bd7533f3209b9fe6893e942e573297ecba453e\",\"message\":\"You Sent:- @[email:'mdhakecha+1@gmail.com']Manish Dhakecha [/email] test note edit  Apr 19, 2021, 05:56 PM\",\"created\":\"2021-04-19T13:37:15.000Z\",\"modified\":\"2021-04-19T14:04:17.000Z\",\"source\":\"Ignite\",\"personId\":\"56575f4e-0eeb-479e-a6a6-e8f1116f884e\",\"jobNimbusId\":null,\"jobNimbusLeadId\":null,\"version\":1,\"propertyType\":0,\"__v\":0},\"replies\":[{\"attachments\":null,\"taggedUsers\":[{\"Key\":0,\"Value\":{\"email\":\"mdhakecha+1@gmail.com\",\"firstName\":\"Manish\",\"lastName\":\"Dhakecha\",\"phone\":null,\"isSendEmail\":true,\"isSendSms\":true,\"userId\":\"18\"}}],\"parentIds\":[\"606d86d0998f280007017ac3\",\"f6bd7533f3209b9fe6893e942e573297ecba453e\"],\"user\":[{\"email\":\"mdhakecha+1@gmail.com\",\"firstName\":\"Manish\",\"lastName\":\"Dhakecha\",\"phone\":null,\"isSendEmail\":true,\"isSendSms\":true,\"userId\":\"18\"}],\"pinnedBy\":[],\"likedBy\":[],\"_id\":\"607d8f5be6e3cb00084a6273\",\"referenceId\":\"606d86d0998f280007017ac3\",\"threadId\":null,\"message\":\"You Sent:- @[email:'mdhakecha+1@gmail.com']Manish Dhakecha [/email] test note comment testing edit comment  Apr 19, 2021, 05:56 PM\",\"created\":\"2021-04-19T14:10:33.000Z\",\"modified\":\"2021-04-19T14:29:37.000Z\",\"source\":\"Ignite\",\"personId\":\"56575f4e-0eeb-479e-a6a6-e8f1116f884e\",\"jobNimbusId\":null,\"jobNimbusLeadId\":null,\"version\":1,\"propertyType\":0,\"__v\":0}]}}"; 

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
                        phone = a.PhoneNumbers?.FirstOrDefault().Number,
                        isSendEmail = true,
                        isSendSms = true,
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
                    isSendEmail = true,
                    isSendSms = true,
                };
            }

            request.AddHeader("Content-Type", "application/json");
            request.AddJsonBody(req);

            var response = client.Execute(request);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                SaveRequest(JsonConvert.SerializeObject(request), response.Content, url, response.StatusCode, null);
                throw new ApplicationException($"AddEditNote Failed. {response.ErrorMessage} {response.StatusCode}");
            }
            try
            {
                SaveRequest(JsonConvert.SerializeObject(request), response.Content, url, response.StatusCode, null);
            }
            catch (Exception)
            {
                throw new ApplicationException($"AddEditNote Failed. {response.StatusCode}");
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
