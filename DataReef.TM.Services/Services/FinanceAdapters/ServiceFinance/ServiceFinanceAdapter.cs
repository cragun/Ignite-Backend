using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.ServiceModel;
using System.ServiceModel.Activation;
using DataReef.Core.Attributes;
using DataReef.TM.Contracts.Services;
using DataReef.TM.DataAccess.Database;
using DataReef.TM.Models.DTOs.FinanceAdapters;
using DataReef.TM.Services.Services;
using DataReef.TM.Services.Services.FinanceAdapters.ServiceFinance;
using DataReef.TM.Services.Services.FinanceAdapters.ServiceFinance.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using RestSharp;
using RestSharp.Authenticators;

namespace DataReef.TM.Services
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(AddressFilterMode = AddressFilterMode.Any)]
    [Service(typeof(IServiceFinanceAdapter))]
    public class ServiceFinanceAdapter : FinancialAdapterBase, IServiceFinanceAdapter
    {
        private ServiceFinanceOuSetting _serviceFinanceOuSetting;

        public ServiceFinanceAdapter(Lazy<IOUSettingService> ouSettingService) : base(ServiceFinanceResources.Name, ouSettingService)
        {
        }

        public override string GetBaseUrl(Guid ouid)
        {
            InitializeSettings();

            return _serviceFinanceOuSetting.BaseUrl;
        }

        public override AuthenticationContext GetAuthenticationContext(Guid ouid)
        {
            return null;
        }

        public override TokenResponse AuthorizeAdapter(AuthenticationContext authenticationContext)
        {
            return new TokenResponse(ConfigurationManager.AppSettings[ServiceFinanceResources.MasterKey]);
        }

        public override Dictionary<string, string> GetCustomHeaders(TokenResponse tokenResponse)
        {
            return new Dictionary<string, string>
            {
                {ServiceFinanceResources.Headers.DatareefKey, tokenResponse.Token},
                {ServiceFinanceResources.Headers.DealerId, _serviceFinanceOuSetting.DealerId}
            };
        }

        public SubmitApplicationResponse SubmitApplication(SubmitApplicationRequest submitRequest)
        {
            var authContext = EnsureInitialized(submitRequest.OUID).Result;

            var tokenResponse = AuthorizeAdapter(authContext);
            var headers = GetCustomHeaders(tokenResponse);

            var payload = GetPayload(submitRequest, headers);
            var payloadJson = JsonConvert.SerializeObject(payload, new IsoDateTimeConverter { DateTimeFormat = "MM/dd/yyyy" });
            var pld = JObject.Parse(payloadJson);

            using (var client = new HttpClient())
            {
                var url = $"{serviceUrl}{ServiceFinanceResources.SubmitApplication}";
                using (var request = new HttpRequestMessage(HttpMethod.Post, url))
                {
                    foreach (var header in headers)
                    {
                        request.Headers.Add(header.Key, header.Value);
                    }

                    var parameters = new List<KeyValuePair<string, string>>();
                    foreach (dynamic entry in pld)
                    {
                        parameters.Add(new KeyValuePair<string, string>(entry.Key.ToString(), entry.Value.ToString()));
                    }

                    using (var content = new FormUrlEncodedContent(parameters))
                    {
                        request.Content = content;
                        var response = client.SendAsync(request).Result;
                        var resultString = response.Content.ReadAsStringAsync().Result;

                        try
                        {
                            SaveRequest(payloadJson, resultString, url, JsonConvert.SerializeObject(request.Headers), null);
                        }
                        catch (Exception)
                        {
                        }

                        if (response.IsSuccessStatusCode)
                        {
                            var result = response.Content.ReadAsAsync<SubmitApplicationResponse>().Result;

                            return result;
                        }

                        return new SubmitApplicationResponse
                        {
                            Error = "Failed to post data to ServiceFinance"
                        };
                    }
                }
            }
        }

        private void InitializeSettings()
        {
            var setting = ouSettings.GetByKey(ServiceFinanceResources.OuSettings.SettingName);
            if (string.IsNullOrWhiteSpace(setting))
                throw new ApplicationException(ServiceFinanceResources.Exceptions.SettingsNotFound);

            _serviceFinanceOuSetting = JsonConvert.DeserializeObject<ServiceFinanceOuSetting>(setting);

            if (string.IsNullOrWhiteSpace(_serviceFinanceOuSetting.BaseUrl))
                throw new ApplicationException(string.Format(ServiceFinanceResources.Exceptions.InvalidSetting, nameof(ServiceFinanceOuSetting.BaseUrl)));
            if (string.IsNullOrWhiteSpace(_serviceFinanceOuSetting.Username))
                throw new ApplicationException(string.Format(ServiceFinanceResources.Exceptions.InvalidSetting, nameof(ServiceFinanceOuSetting.Username)));
            if (string.IsNullOrWhiteSpace(_serviceFinanceOuSetting.Password))
                throw new ApplicationException(string.Format(ServiceFinanceResources.Exceptions.InvalidSetting, nameof(ServiceFinanceOuSetting.Password)));
            if (string.IsNullOrWhiteSpace(_serviceFinanceOuSetting.DealerId))
                throw new ApplicationException(string.Format(ServiceFinanceResources.Exceptions.InvalidSetting, nameof(ServiceFinanceOuSetting.DealerId)));
            if (string.IsNullOrWhiteSpace(_serviceFinanceOuSetting.DealerName))
                throw new ApplicationException(string.Format(ServiceFinanceResources.Exceptions.InvalidSetting, nameof(ServiceFinanceOuSetting.DealerName)));
        }

        private SubmitApplicationPostRequest GetPayload(SubmitApplicationRequest submitRequest, Dictionary<string, string> headers)
        {
            var payload = new SubmitApplicationPostRequest(submitRequest)
            {
                DealerNumber = headers[ServiceFinanceResources.Headers.DealerId],
                DealerName = _serviceFinanceOuSetting.DealerName
            };

            using (var context = new DataContext())
            {
                var planDefinition = context.FinancePlaneDefinitions.FirstOrDefault(d => d.Guid == submitRequest.PlanDefinitionID);
                if (planDefinition?.TagString == null)
                    throw new ApplicationException(ServiceFinanceResources.Exceptions.InvalidPlanDefinition);

                var adapterSettings = JsonConvert.DeserializeObject<List<AdapterPlanDefinitionSetting>>(planDefinition.TagString);
                var financeAdapterSetting = adapterSettings.FirstOrDefault(s => s.AdapterName == ServiceFinanceResources.Name);
                if (financeAdapterSetting?.Settings == null)
                    throw new ApplicationException(ServiceFinanceResources.Exceptions.InvalidPlanDefinitionSetting);

                var programType = financeAdapterSetting.Settings.First().ProgramType;
                payload.ProgramType = programType;
            }

            return payload;
        }
    }
}
