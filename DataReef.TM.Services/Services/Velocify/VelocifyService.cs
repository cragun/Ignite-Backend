using System;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Xml;
using System.Data.Entity;
using DataReef.TM.Contracts.Services;
using System.ServiceModel.Activation;
using System.ServiceModel;
using DataReef.TM.DataAccess.Database;
using DataReef.TM.Models;
using DataReef.TM.Models.PRMI;
using Newtonsoft.Json;
using DataReef.Core.Infrastructure.Authorization;
using System.Threading.Tasks;

namespace DataReef.TM.Services.Services
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(AddressFilterMode = AddressFilterMode.Any)]
    public class VelocifyService : IVelocifyService
    {
        private readonly IOUSettingService _ouSettingService;
        private readonly string _velocifyUrl;
        private readonly string _velocifyProvider;
        private readonly string _velocifyClient;
        private readonly string _velocifyCampaignId;
        private readonly string _velocifyXmlResponse;
        private readonly string _velocifyAccrossCampaigns;

        public VelocifyService(IOUSettingService ouSettingService)
        {
            _ouSettingService = ouSettingService;
            _velocifyUrl = ConfigurationManager.AppSettings[VelocifySettings.VelocifyUrl];
            _velocifyProvider = ConfigurationManager.AppSettings[VelocifySettings.VelocifyProvider];
            _velocifyClient = ConfigurationManager.AppSettings[VelocifySettings.VelocifyClient];
            _velocifyCampaignId = ConfigurationManager.AppSettings[VelocifySettings.VelocifyCampaignId];
            _velocifyXmlResponse = ConfigurationManager.AppSettings[VelocifySettings.VelocifyXmlResponse];
            _velocifyAccrossCampaigns = ConfigurationManager.AppSettings[VelocifySettings.VelocifyAccrossCampaigns];
        }

        public VelocifyResponse SendProposal(Guid? ouID, VelocifyRequest request)
        {
            ValidateConfiguration();

            using (var context = new DataContext())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        var dbRequest = context.VelocifyRequests.FirstOrDefault(r => r.ReferenceID == request.ReferenceID);
                        if (dbRequest != null)
                        {
                            context.VelocifyRequests.Remove(dbRequest);
                            var dbResponse = context.VelocifyResponses.FirstOrDefault(r => r.ReferenceID == request.ReferenceID);
                            if (dbResponse != null)
                            {
                                context.VelocifyResponses.Remove(dbResponse);
                            }
                        }

                        if (string.IsNullOrEmpty(request.SalesRepEmail))
                        {
                            request.SalesRepEmail = context.People.FirstOrDefault(p => p.Guid == SmartPrincipal.UserId)?.EmailAddressString;
                        }

                        if (string.IsNullOrEmpty(request.SalesRepCompanyName)
                            && ouID.HasValue
                            && ouID.Value != Guid.Empty)
                        {
                            request.SalesRepCompanyName = context
                                    .OUs
                                    .FirstOrDefault(o => o.Guid == ouID.Value)?
                                    .Name;
                        }

                        context.VelocifyRequests.Add(request);
                        string url = null;

                        var proposalResponse = PostProposalRequest(request, out url);
                        var result = GetVelocifyResponse(proposalResponse, request);

                        request.TagString = url;

                        context.VelocifyResponses.Add(result);
                        context.SaveChanges();

                        transaction.Commit();

                        return result;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw new Exception("Failed to post velocify request", ex);
                    }
                }
            }
        }

        private VelocifyResponse GetVelocifyResponse(HttpResponseMessage proposalResponse, VelocifyRequest request)
        {
            var response = new VelocifyResponse { Message = "Success" };
            if (!proposalResponse.IsSuccessStatusCode)
            {
                response.Message = $"Failed to post velocify request: {proposalResponse.StatusCode}";
                return response;
            }

            bool responseIsXml;
            if (!bool.TryParse(_velocifyXmlResponse, out responseIsXml) || !responseIsXml)
            {
                return response;
            }

            var responseString = proposalResponse.Content.ReadAsStringAsync().Result;
            var xmlResponse = new XmlDocument();
            xmlResponse.LoadXml(responseString);

            var importResults = xmlResponse.GetElementsByTagName("ImportResult");
            var importResult = importResults[0];

            var attributes = importResult.Attributes;
            if (attributes == null)
                return response;

            response.VelocifyRequestID = request.Guid;
            response.ReferenceID = request.ReferenceID;
            response.LeadID = attributes["leadId"].Value;
            response.Result = attributes["result"].Value;
            response.Message = attributes["message"].Value;

            return response;
        }

        private void ValidateConfiguration()
        {
            if (string.IsNullOrEmpty(_velocifyUrl))
                throw new Exception("Invalid velocify url");
            if (string.IsNullOrEmpty(_velocifyProvider))
                throw new Exception("Invalid velocify provider");
            if (string.IsNullOrEmpty(_velocifyClient))
                throw new Exception("Invalid velocify client");
            if (string.IsNullOrEmpty(_velocifyCampaignId))
                throw new Exception("Invalid velocify campaign id");
            if (string.IsNullOrEmpty(_velocifyXmlResponse))
                throw new Exception("Invalid velocify xml response");
            if (string.IsNullOrEmpty(_velocifyAccrossCampaigns))
                throw new Exception("Invalid velocify across campaigns");
        }

        private HttpResponseMessage PostProposalRequest(VelocifyRequest request, out string url)
        {
            using (var httpClient = new HttpClient())
            {
                url = GetVelocifyUrl(request);
                var response = httpClient.PostFormUrlEncodedAsync(url, request);

                return response;
            }
        }

        private string GetVelocifyUrl(VelocifyRequest request)
        {
            string url = $"{_velocifyUrl}?Provider={_velocifyProvider}&Client={_velocifyClient}&CampaignId={_velocifyCampaignId}&XmlResponse={_velocifyXmlResponse}&AcrossCampaigns={_velocifyAccrossCampaigns}";

            var ouSettings = _ouSettingService.List(filter: $"name=Contractor ID&value={request.DealerName}").ToList();
            if (!ouSettings.Any()) return url;

            var ouid = ouSettings.FirstOrDefault().OUID;
            var velocifySettings = Task.Run(() => _ouSettingService.GetSettings(ouid, OUSettingGroupType.HiddenConfigs)).Result;
            if (!velocifySettings.ContainsKey(VelocifyOuSettings.VelocifyBaseAddress) ||
                !velocifySettings.ContainsKey(VelocifyOuSettings.VelocifyParameters))
                return url;

            var baseAddress = velocifySettings.FirstOrDefault(s => s.Key.Equals(VelocifyOuSettings.VelocifyBaseAddress, StringComparison.InvariantCultureIgnoreCase)).Value.Value;
            var parameters = velocifySettings.FirstOrDefault(s => s.Key.Equals(VelocifyOuSettings.VelocifyParameters, StringComparison.CurrentCultureIgnoreCase)).Value.Value;

            url = $"{baseAddress}?XmlResponse=true";

            dynamic parametersJson = JsonConvert.DeserializeObject(parameters);
            foreach (dynamic entry in parametersJson)
            {
                url = $"{url}&{entry.Name}={entry.Value}";
            }

            return url;
        }
    }
}
