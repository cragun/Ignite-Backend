using DataReef.Core;
using DataReef.Core.Logging;
using DataReef.TM.Api.Bootstrap;
using DataReef.TM.Api.Classes;
using DataReef.TM.Api.JsonFormatter;
using DataReef.TM.Api.Security;
using DataReef.TM.Contracts.Services;
using Microsoft.Practices.ServiceLocation;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace DataReef.TM.Api
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            var appXmlType = config.Formatters.XmlFormatter.SupportedMediaTypes.FirstOrDefault(t => t.MediaType == "application/xml");
            config.Formatters.XmlFormatter.SupportedMediaTypes.Remove(appXmlType);

            var json = config.Formatters.JsonFormatter;
            json.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Serialize;
            json.SerializerSettings.PreserveReferencesHandling = PreserveReferencesHandling.None;
            json.SerializerSettings.Formatting = Formatting.None;
            json.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
            json.SerializerSettings.ContractResolver = new DeltaContractResolver(json);
            json.SerializerSettings.Converters = new List<JsonConverter>
            {
                new GuidRefJsonConverter(),
                new SyncItemJsonConverter()
            };
            // config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;

            // Web API routes
            config.MapHttpAttributeRoutes(new GenericDirectRouteProvider());
            // config.MessageHandlers.Add(new LogRequestAndResponseHandler());
            config.Filters.Add(new UserFriendlyExceptionFilterAttribute());
            

             // Authentication and Authorization
             var logger = ServiceLocator.Current.GetInstance<ILogger>();
            var authenticationService = ServiceLocator.Current.GetInstance<IAuthenticationService>();
            var appSettingService = ServiceLocator.Current.GetInstance<IAppSettingService>();
            GlobalConfiguration.Configuration.MessageHandlers.Add(new CorsValidationHandler()); // consider replacing with CorsMessageHandler and use ICorsPolicyProviderFactory
            GlobalConfiguration.Configuration.MessageHandlers.Add(new TokenValidationHandler(logger));
            GlobalConfiguration.Configuration.MessageHandlers.Add(new ClaimsTransformationHandler());
            GlobalConfiguration.Configuration.MessageHandlers.Add(new VersionValidationHandler(logger, appSettingService));
            GlobalConfiguration.Configuration.Filters.Add(new CustomAuthorizationFilter(authenticationService));
        }
    }
}