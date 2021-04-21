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
            //response.Content = "{\"9f54a513243a2dd15e3b558d44581a566fb6fdf1\":{\"notes\":{\"attachments\":null,\"taggedUsers\":[{\"Key\":0,\"Value\":{\"email\":\"mdhakecha+1@gmail.com\",\"firstName\":\"Manish\",\"lastName\":\"Dhakecha\",\"phone\":null,\"isSendEmail\":true,\"isSendSms\":true,\"userId\":\"18\"}}],\"parentIds\":[\"607d76cc963e32000756d110\"],\"user\":[{\"email\":\"mdhakecha+1@gmail.com\",\"firstName\":\"Manish\",\"lastName\":\"Dhakecha\",\"phone\":null,\"isSendEmail\":true,\"isSendSms\":true,\"userId\":\"18\"}],\"pinnedBy\":[],\"likedBy\":[],\"_id\":\"607e67a9239a51000854b9de\",\"referenceId\":\"607d76cc963e32000756d110\",\"threadId\":\"9f54a513243a2dd15e3b558d44581a566fb6fdf1\",\"message\":\"[/email]\",\"created\":\"2021-04-20T05:33:27.000Z\",\"modified\":\"2021-04-20T09:38:21.000Z\",\"source\":\"Ignite\",\"personId\":\"56575f4e-0eeb-479e-a6a6-e8f1116f884e\",\"jobNimbusId\":null,\"jobNimbusLeadId\":null,\"version\":1,\"propertyType\":0,\"__v\":0}},\"969cc264843f1fa96d13b09f8b2537d0a6f9163d\":{\"notes\":{\"attachments\":null,\"taggedUsers\":[{\"Key\":0,\"Value\":{\"email\":\"mdhakecha+1@gmail.com\",\"firstName\":\"Manish\",\"lastName\":\"Dhakecha\",\"phone\":null,\"isSendEmail\":true,\"isSendSms\":true,\"userId\":\"18\"}}],\"parentIds\":[\"607d76cc963e32000756d110\"],\"user\":[{\"email\":\"mdhakecha+1@gmail.com\",\"firstName\":\"Manish\",\"lastName\":\"Dhakecha\",\"phone\":null,\"isSendEmail\":true,\"isSendSms\":true,\"userId\":\"18\"}],\"pinnedBy\":[],\"likedBy\":[],\"_id\":\"607e56f6fce0500008cb783f\",\"referenceId\":\"607d76cc963e32000756d110\",\"threadId\":\"969cc264843f1fa96d13b09f8b2537d0a6f9163d\",\"message\":\"You Sent:-\\n\\n@[email:'mdhakecha+1@gmail.com']Manish Dhakecha [/email] hey\\n\\n Apr 20, 2021, 09:52 AM\",\"created\":\"2021-04-20T04:22:11.000Z\",\"modified\":null,\"source\":\"Ignite\",\"personId\":\"56575f4e-0eeb-479e-a6a6-e8f1116f884e\",\"jobNimbusId\":null,\"jobNimbusLeadId\":null,\"version\":1,\"propertyType\":0,\"__v\":0},\"replies\":[{\"attachments\":[\"(\\n  \\\"\\\"\\n)\"],\"taggedUsers\":[],\"parentIds\":[\"607d76cc963e32000756d110\",\"969cc264843f1fa96d13b09f8b2537d0a6f9163d\"],\"user\":[{\"email\":\"mdhakecha+1@gmail.com\",\"firstName\":\"Manish\",\"lastName\":\"Dhakecha\",\"phone\":null,\"isSendEmail\":true,\"isSendSms\":true,\"userId\":\"18\"}],\"pinnedBy\":[],\"likedBy\":[],\"_id\":\"607ed3f6e4a86300086f8618\",\"referenceId\":\"607d76cc963e32000756d110\",\"threadId\":null,\"message\":\"(null) Sent:-\\n\\n@Manish Dhakecha Hey Test Reply \\n\\n Apr 19, 2021, 07:18 PM\\n\\n \\nYou Replied:- \\n\\n @(null) test\\n\\nApr 19, 2021, 07:18 PM\",\"created\":\"2021-04-20T13:15:33.000Z\",\"modified\":null,\"source\":\"Ignite\",\"personId\":\"56575f4e-0eeb-479e-a6a6-e8f1116f884e\",\"jobNimbusId\":null,\"jobNimbusLeadId\":null,\"version\":1,\"propertyType\":0,\"__v\":0}]},\"c532ec99e030b3add06eed7c0761d39821010c71\":{\"notes\":{\"attachments\":null,\"taggedUsers\":[{\"Key\":0,\"Value\":{\"email\":\"mdhakecha+1@gmail.com\",\"firstName\":\"Manish\",\"lastName\":\"Dhakecha\",\"phone\":null,\"isSendEmail\":true,\"isSendSms\":true,\"userId\":\"18\"}}],\"parentIds\":[\"607d76cc963e32000756d110\"],\"user\":[{\"email\":\"mdhakecha+1@gmail.com\",\"firstName\":\"Manish\",\"lastName\":\"Dhakecha\",\"phone\":null,\"isSendEmail\":true,\"isSendSms\":true,\"userId\":\"18\"}],\"pinnedBy\":[],\"likedBy\":[],\"_id\":\"607d8a389484f200087d749b\",\"referenceId\":\"607d76cc963e32000756d110\",\"threadId\":\"c532ec99e030b3add06eed7c0761d39821010c71\",\"message\":\"(null) Sent:-\\n\\n@Manish Dhakecha Hey\\n\\n Apr 19, 2021, 07:18 PM\\n\\n \\nYou Replied:- \\n\\n @(null) test\\n\\nApr 19, 2021, 07:18 PM\",\"created\":\"2021-04-19T13:48:40.000Z\",\"modified\":\"2021-04-20T12:19:51.000Z\",\"source\":\"Ignite\",\"personId\":\"56575f4e-0eeb-479e-a6a6-e8f1116f884e\",\"jobNimbusId\":null,\"jobNimbusLeadId\":null,\"version\":1,\"propertyType\":0,\"__v\":0},\"replies\":[{\"attachments\":[\"(\\n  \\\"\\\"\\n)\"],\"taggedUsers\":[],\"parentIds\":[\"607d76cc963e32000756d110\",\"c532ec99e030b3add06eed7c0761d39821010c71\"],\"user\":[{\"email\":\"mdhakecha+1@gmail.com\",\"firstName\":\"Manish\",\"lastName\":\"Dhakecha\",\"phone\":null,\"isSendEmail\":true,\"isSendSms\":true,\"userId\":\"18\"}],\"pinnedBy\":[],\"likedBy\":[],\"_id\":\"607eccab9c24ae0008e159f0\",\"referenceId\":\"607d76cc963e32000756d110\",\"threadId\":null,\"message\":\"(null) Sent:-\\n\\n@Manish Dhakecha Hey\\n\\n Apr 19, 2021, 07:18 PM\\n\\n \\nYou Replied:- \\n\\n @(null) test\\n\\nApr 19, 2021, 07:18 PM\",\"created\":\"2021-04-20T12:44:25.000Z\",\"modified\":null,\"source\":\"Ignite\",\"personId\":\"56575f4e-0eeb-479e-a6a6-e8f1116f884e\",\"jobNimbusId\":null,\"jobNimbusLeadId\":null,\"version\":1,\"propertyType\":0,\"__v\":0}]},\"9fd516d7c529637a177eb58366693db6a5df63df\":{\"notes\":{\"attachments\":null,\"taggedUsers\":[{\"Key\":0,\"Value\":{\"email\":\"mdhakecha+1@gmail.com\",\"firstName\":\"Manish\",\"lastName\":\"Dhakecha\",\"phone\":null,\"isSendEmail\":true,\"isSendSms\":true,\"userId\":\"18\"}}],\"parentIds\":[\"607d76cc963e32000756d110\"],\"user\":[{\"email\":\"mdhakecha+1@gmail.com\",\"firstName\":\"Manish\",\"lastName\":\"Dhakecha\",\"phone\":null,\"isSendEmail\":true,\"isSendSms\":true,\"userId\":\"18\"}],\"pinnedBy\":[],\"likedBy\":[],\"_id\":\"607d89ec9484f200087d749a\",\"referenceId\":\"607d76cc963e32000756d110\",\"threadId\":\"9fd516d7c529637a177eb58366693db6a5df63df\",\"message\":\"You Sent:-\\n\\n@[email:'mdhakecha+1@gmail.com']Manish Dhakecha [/email] Test note\\n\\n Apr 19, 2021, 07:17 PM\",\"created\":\"2021-04-19T13:47:23.000Z\",\"modified\":null,\"source\":\"Ignite\",\"personId\":\"56575f4e-0eeb-479e-a6a6-e8f1116f884e\",\"jobNimbusId\":null,\"jobNimbusLeadId\":null,\"version\":1,\"propertyType\":0,\"__v\":0}}}";

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
