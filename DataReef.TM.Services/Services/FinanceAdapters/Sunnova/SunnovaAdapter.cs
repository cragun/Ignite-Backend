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
using DataReef.TM.Contracts.Services;
using System.Linq;
using System.Threading.Tasks;

namespace DataReef.TM.Services.Services.FinanceAdapters.Sunnova
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(AddressFilterMode = AddressFilterMode.Any)]
    [Service(typeof(ISunnovaAdapter))]
    public class SunnovaAdapter : FinancialAdapterBase, ISunnovaAdapter
    {
        private static readonly string url = System.Configuration.ConfigurationManager.AppSettings["Sunnova.test.url"];
        private static readonly string AuthUsername = System.Configuration.ConfigurationManager.AppSettings["Sunnova.Auth.Username"];
        private static readonly string AuthPassword = System.Configuration.ConfigurationManager.AppSettings["Sunnova.Auth.Password"];
        private static readonly string ReturnUrl = System.Configuration.ConfigurationManager.AppSettings["Sunnova.test.ReturnUrl"];

        public SunnovaAdapter(Lazy<IOUSettingService> ouSettingService) : base("Sunnova", ouSettingService)
        {
        }

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

        public class SunnovaProjects
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Middle_Name { get; set; }
            public string Email { get; set; }
            public string Preferred_Contact_Method { get; set; }
            public string Preferred_Language { get; set; }
            public Phone Phone { get; set; }
            public Addresss Address { get; set; }
            public string external_id { get; set; }
        }

        public class SunnovaCredit
        {

        }

        public async Task<string> GetSunnovaToken()
        {
            try
            {
                string svcCredentials = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(AuthUsername + ":" + AuthPassword));

                var request = new RestRequest($"/authentication", Method.GET);
                request.AddHeader("Authorization", "Basic " + svcCredentials);
                var response = await client.ExecuteTaskAsync(request);

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


        public async Task<List<SunnovaLead>> CreateSunnovaLead(Property property)
        {
            using (var dc = new DataContext())
            {
                SunnovaProjects req = new SunnovaProjects();

                string number = property.GetMainPhoneNumber()?.Replace("-", "");
                string email = property.GetMainEmailAddress();

                Phone phone = new Phone();
                phone.Number = number == null ? "" : number;
                // number is required
                phone.Type = "Mobile";

                Addresss addr = new Addresss();
                addr.City = property.City;
                addr.Country = "USA";
                addr.Latitude = Convert.ToDouble(String.Format("{0:0.0000}", property.Latitude));
                addr.Longitude = Convert.ToDouble(String.Format("{0:0.0000}", property.Longitude));
                addr.PostalCode = property.ZipCode;
                addr.State = property.State == null ? "" : property.State;
                // state is required
                addr.Street = property.Address1 == null ? "" : property.Address1;

                var name = property.GetMainOccupant();

                req.FirstName = name?.FirstName == null ? "" : name?.FirstName;
                req.LastName = name?.LastName == null ? "" : name?.LastName;
                req.Middle_Name = name?.MiddleInitial == null ? "" : name?.MiddleInitial;
                req.Email = email == null ? "" : email;
                req.Phone = phone;
                req.Address = addr;
                req.external_id = property.ExternalID == null ? "" : property.ExternalID;
                req.Preferred_Contact_Method = "Email";
                req.Preferred_Language = "English";
                // req.Suffix = "";

                string token = await GetSunnovaToken();
                var request = new RestRequest($"/services/v1.0/leads", Method.POST);
                request.AddJsonBody(req);
                request.AddHeader("Authorization", "Bearer " + token);

                var response = await client.ExecuteTaskAsync(request);
                //var json = new JavaScriptSerializer().Serialize(req);
                //var resp = new JavaScriptSerializer().Serialize(response);                


                if (response.StatusCode != HttpStatusCode.Created)
                {
                    SaveRequest(JsonConvert.SerializeObject(request), response, url + "/services/v1.0/leads", null, token);
                    throw new ApplicationException($"CreateSunnovaLead Failed. {response.Content}");
                }

                try
                {
                    SaveRequest(JsonConvert.SerializeObject(request), response, url + "/services/v1.0/leads", null, token);
                }
                catch (Exception)
                {
                }

                var content = response.Content;
                var ret = JsonConvert.DeserializeObject<List<SunnovaLead>>(content);
                string SunnovaLeadID = ret.FirstOrDefault() != null ? ret.FirstOrDefault().lead.Id : "";

                if (!string.IsNullOrEmpty(SunnovaLeadID))
                {
                    var resp = await GetSunnovaContacts(SunnovaLeadID);

                    property.SunnovaLeadID = SunnovaLeadID;
                    property.SunnovaContactsResponse = new JavaScriptSerializer().Serialize(resp);
                    await dc.SaveChangesAsync();
                }

                return ret;
            }
        }


        public async Task<List<SunnovaContacts>> GetSunnovaContacts(string SunnovaLeadID)
        {
            using (var dc = new DataContext())
            {
                //https://apitest.sunnova.com/services/v1.0/leads/{LeadID}/contacts
                string token = await GetSunnovaToken();
                var request = new RestRequest($"/services/v1.0/leads/" + SunnovaLeadID + "/contacts", Method.GET);
                request.AddHeader("Authorization", "Bearer " + token);

                var response = await client.ExecuteTaskAsync(request);

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new ApplicationException($"GetSunnovaContacts Failed. {response.Content}");
                }

                try
                {
                    SaveRequest(JsonConvert.SerializeObject(request), response, url + "/services/v1.0/leads/" + SunnovaLeadID + "/contacts", null, token);
                }
                catch (Exception)
                {
                }

                var content = response.Content;
                var ret = JsonConvert.DeserializeObject<List<SunnovaContacts>>(content);
                return ret;
            }
        }

        public async Task<SunnovaLeadCreditResponse> PassSunnovaLeadCredit(Property property)
        {
            if (!string.IsNullOrEmpty(property.SunnovaContactsResponse))
            {
                SunnovaLeadCredit SunnovaContact = JsonConvert.DeserializeObject<SunnovaLeadCredit>(property.SunnovaContactsResponse);

                var SunnovaContactId = SunnovaContact.Contacts;
                SunnovaCredit req = new SunnovaCredit();

                string token = await GetSunnovaToken();
                var request = new RestRequest($"/services/v1.0/contacts/" + SunnovaContactId.id + "/credit?action=email", Method.PATCH);
                request.AddJsonBody(req);
                request.AddHeader("Authorization", "Bearer " + token);

                var response = await client.ExecuteTaskAsync(request);

                if (response.StatusCode != HttpStatusCode.Created)
                {
                    SaveRequest(JsonConvert.SerializeObject(request), response, url + "/services/v1.0/contacts/" + SunnovaContactId.id + "/credit?action=email", null, token);
                    throw new ApplicationException($"passSunnovaLeadCredit Failed. {response.Content}");
                }

                try
                {
                    SaveRequest(JsonConvert.SerializeObject(request), response, url + "/services/v1.0/contacts/" + SunnovaContactId.id + "/credit?action=email", null, token);
                }
                catch (Exception)
                {
                }

                var content = response.Content;
                var ret = JsonConvert.DeserializeObject<SunnovaLeadCreditResponse>(content);

                return ret;
            }

            return new SunnovaLeadCreditResponse();
        }

        public async Task<SunnovaLeadCreditResponseData> PassSunnovaLeadCreditURL(Property property)
        {
            if (!string.IsNullOrEmpty(property.SunnovaContactsResponse))
            {
                SunnovaLeadCredit SunnovaContact = JsonConvert.DeserializeObject<SunnovaLeadCredit>(property.SunnovaContactsResponse);

                var SunnovaContactId = SunnovaContact.Contacts;
                SunnovaLeadCreditRequestData req = new SunnovaLeadCreditRequestData();
                req.Return_URL = ReturnUrl;

                string token = await GetSunnovaToken();
                var request = new RestRequest($"/services/v1.0/contacts/" + SunnovaContactId.id + "/credit?action=inperson", Method.PATCH);
                request.AddJsonBody(req);
                request.AddHeader("Authorization", "Bearer " + token);

                var response = await client.ExecuteTaskAsync(request);

                if (response.StatusCode != HttpStatusCode.Created)
                {
                    SaveRequest(JsonConvert.SerializeObject(request), response, url + "/services/v1.0/contacts/" + SunnovaContactId.id + "/credit?action=inperson", null, token);
                    throw new ApplicationException($"passSunnovaLeadCredit Failed. {response.Content}");
                }

                try
                {
                    SaveRequest(JsonConvert.SerializeObject(request), response, url + "/services/v1.0/contacts/" + SunnovaContactId.id + "/credit?action=inperson", null, token);
                }
                catch (Exception)
                {
                }

                var content = response.Content;
                var ret = JsonConvert.DeserializeObject<SunnovaLeadCreditResponseData>(content);

                return ret;
            }
            return new SunnovaLeadCreditResponseData();
        }



        public async Task<string> GetSunnovaCreditURL(Property property)
        {
            if (property != null && property.SunnovaLeadID == null)
            {
                await CreateSunnovaLead(property);
                using (var dc = new DataContext())
                {
                    property =  dc.Properties.FirstOrDefault(x => x.Guid == property.Guid && !x.IsDeleted);
                }
            }

            if (!string.IsNullOrEmpty(property.SunnovaContactsResponse))
            {
                SunnovaLeadCredit SunnovaContact = JsonConvert.DeserializeObject<SunnovaLeadCredit>(property.SunnovaContactsResponse);

                var SunnovaContactId = SunnovaContact.Contacts;
                SunnovaLeadCreditRequestData req = new SunnovaLeadCreditRequestData();
                req.Return_URL = ReturnUrl;

                string token = await GetSunnovaToken();
                var request = new RestRequest($"/services/v1.0/contacts/" + SunnovaContactId.id + "/credit?action=inperson", Method.PATCH);
                request.AddJsonBody(req);
                request.AddHeader("Authorization", "Bearer " + token);

                var response = await client.ExecuteTaskAsync(request);

                if (response.StatusCode != HttpStatusCode.Created)
                {
                    SaveRequest(JsonConvert.SerializeObject(request), response, url + "/services/v1.0/contacts/" + SunnovaContactId.id + "/credit?action=inperson", null, token);
                    throw new ApplicationException($"passSunnovaLeadCredit Failed. {response.Content}");
                }

                try
                {
                    SaveRequest(JsonConvert.SerializeObject(request), response, url + "/services/v1.0/contacts/" + SunnovaContactId.id + "/credit?action=inperson", null, token);
                }
                catch (Exception)
                {
                }

                var content = response.Content;
                var ret = JsonConvert.DeserializeObject<SunnovaLeadCreditResponseData>(content);

                return ret?.Signing_URL.ToString() ?? string.Empty;
            }
            return string.Empty;
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
