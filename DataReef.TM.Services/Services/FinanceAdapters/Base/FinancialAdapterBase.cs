using DataReef.Integrations.Core;
using DataReef.TM.Contracts.Services;
using DataReef.TM.DataAccess.Database;
using DataReef.TM.Models.DataViews;
using DataReef.TM.Models.Enums;
using DataReef.TM.Models.FinancialIntegration;
using DataReef.TM.Services.Services;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Serializers;
using System;
using System.Collections.Generic;

namespace DataReef.TM.Services
{
    public abstract class FinancialAdapterBase : IntegrationProviderBase
    {
        protected readonly Lazy<IOUSettingService> _ouSettingService;
        protected readonly string AdapterName;
        protected Dictionary<string, ValueTypePair<SettingValueType, string>> ouSettings;

        protected FinancialAdapterBase(string adapterName, Lazy<IOUSettingService> ouSettingService) : base("none")
        {
            _ouSettingService = ouSettingService;
            AdapterName = adapterName;
        }

        protected AuthenticationContext EnsureInitialized(Guid ouid)
        {
            OUID = ouid;
            if (ouSettings != null)
                return GetAuthenticationContext(ouid);

            ouSettings = _ouSettingService.Value.GetSettings(ouid, null);
            if (string.IsNullOrWhiteSpace(serviceUrl) || serviceUrl.Equals("none", StringComparison.InvariantCultureIgnoreCase))
                serviceUrl = GetBaseUrl(ouid);

            return GetAuthenticationContext(ouid);
        }

        public abstract string GetBaseUrl(Guid ouid);

        public abstract AuthenticationContext GetAuthenticationContext(Guid ouid);

        public abstract TokenResponse AuthorizeAdapter(AuthenticationContext authenticationContext);

        public abstract Dictionary<string, string> GetCustomHeaders(TokenResponse tokenResponse);

        public virtual T MakeRequest<T>(Guid ouid, string resource, object payload, List<Parameter> parameters = null, ISerializer serializer = null) where T : class, new()
        {
            var authContext = EnsureInitialized(ouid);

            var tokenResponse = AuthorizeAdapter(authContext);
            var headers = GetCustomHeaders(tokenResponse);
            var response = MakeRequest<T>(serviceUrl, resource, Method.POST, headers, payload, parameters, serializer: serializer);

            return response;
        }

        public virtual string MakeRequest(Guid ouid, string resource, object payload, List<Parameter> parameters = null, ISerializer serializer = null, Method method = Method.POST)
        {
            var authContext = EnsureInitialized(ouid);

            var tokenResponse = AuthorizeAdapter(authContext);
            var headers = GetCustomHeaders(tokenResponse);
            var response = MakeRequest(serviceUrl, resource, method, headers, payload, parameters, serializer: serializer);

            return response;
        }

        protected void SaveRequest(string request, string response, string url, string headers)
        {
            var adapterRequest = new AdapterRequest
            {
                AdapterName = AdapterName,
                Request = request,
                Response = response,
                Url = url,
                Headers = headers
            };

            using (var context = new DataContext())
            {
                context.AdapterRequests.Add(adapterRequest);
                context.SaveChanges();
            }
        }

        protected void SaveRequest(object request, object response, string url, object headers)
        {
            var headersString = headers == null ? null : JsonConvert.SerializeObject(headers);
            var requestString = request == null ? null : JsonConvert.SerializeObject(request);
            var responseString = response == null ? null : JsonConvert.SerializeObject(response);

            SaveRequest(requestString, responseString, url, headersString);
        }
    }
}
