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

        //To create referenceId for leads send from Ignite
        public NoteResponse GetLeadReferenceId(Property property, string apikey)
        { 
            AddLeadReferenceRequest req = new AddLeadReferenceRequest();
            req.lead_reference = new lead_reference() { ignite_id = Convert.ToString(property.Guid), smartboard_id = Convert.ToString(property.SmartBoardId) };
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
                throw new ApplicationException($"GetLeadReference Failed. {response.ErrorMessage} {response.StatusCode}");
            }
            try
            {
                SaveRequest(JsonConvert.SerializeObject(request), response.Content, url, response.StatusCode, null);
            }
            catch (Exception)
            {
                throw new ApplicationException($"GetLeadReference Failed. {response.StatusCode}");
            }

            return JsonConvert.DeserializeObject<NoteResponse>(response.Content);
        }

        //To create notes
        public async Task<NoteResponse> AddNote(string ReferenceId, SBNoteDTO note, Person user)
        {
            NotesRequest req = new NotesRequest();
            req.referenceId = ReferenceId;
            req.message = note.Content;
            req.created = Convert.ToString(note.DateCreated);
            req.source = "Ignite";
            req.attachments = note.Attachments.Split(',').ToList();
            req.taggedUsers = note.TaggedUsers.Select(a => new NoteTaggedUser
            {
                email = a.email,
                phone = a.PhoneNumber,
                isSendEmail = a.IsSendEmail,
                isSendSms = a.IsSendSMS,
                userId = Convert.ToString(a.SmartBoardId),
                firstName = a.FirstName,
                lastName = a.LastName
            }).ToList();

            req.user = new NoteTaggedUser()
            {
                userId = Convert.ToString(note.UserID),
                email = note.Email,
                firstName = user.FirstName,
                lastName = user.LastName,
                isSendEmail = note.IsSendEmail,
                isSendSms = note.IsSendSMS,
                phone = user.PhoneNumbers?.FirstOrDefault()?.Number,
            };

            var request = new RestRequest($"/notes", Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddJsonBody(req);
            var response = await client.ExecuteTaskAsync(request);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                SaveRequest(JsonConvert.SerializeObject(request), response.Content, url, response.StatusCode, null);
                throw new ApplicationException($"CreateNote Failed. {response.ErrorMessage} {response.StatusCode}");
            }
            try
            {
                SaveRequest(JsonConvert.SerializeObject(request), response.Content, url, response.StatusCode, null);
            }
            catch (Exception)
            {
                throw new ApplicationException($"CreateNote Failed. {response.StatusCode}");
            }

            return JsonConvert.DeserializeObject<NoteResponse>(response.Content);
        }

        //To update a note
        public async Task<NoteResponse> UpdateNote(string ReferenceId, SBNoteDTO note)
        {
            NotesRequest req = new NotesRequest();
            req.message = note.Content;
            req.modified = Convert.ToString(note.DateLastModified);

            var request = new RestRequest($"/notes/{ReferenceId}", Method.PATCH);
            request.AddHeader("Content-Type", "application/json");
            request.AddJsonBody(req);
            var response = await client.ExecuteTaskAsync(request);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                SaveRequest(JsonConvert.SerializeObject(request), response.Content, url, response.StatusCode, null);
                throw new ApplicationException($"UpdateNote Failed. {response.ErrorMessage} {response.StatusCode}");
            }
            try
            {
                SaveRequest(JsonConvert.SerializeObject(request), response.Content, url, response.StatusCode, null);
            }
            catch (Exception)
            {
                throw new ApplicationException($"UpdateNote Failed. {response.StatusCode}");
            }

            return JsonConvert.DeserializeObject<NoteResponse>(response.Content);
        }

        //To get a leads note
        public async Task<NoteResponse> GetNote(string ReferenceId)
        {
            var request = new RestRequest($"/notes/{ReferenceId}", Method.GET);
            request.AddHeader("Content-Type", "application/json");

            var response = await client.ExecuteTaskAsync(request);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                SaveRequest(JsonConvert.SerializeObject(request), response.Content, url, response.StatusCode, null);
                throw new ApplicationException($"GetNote Failed. {response.ErrorMessage} {response.StatusCode}");
            }
            try
            {
                SaveRequest(JsonConvert.SerializeObject(request), response.Content, url, response.StatusCode, null);
            }
            catch (Exception)
            {
                throw new ApplicationException($"GetNote Failed. {response.StatusCode}");
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
