using System.Web.Http;
using DataReef.TM.ClientApi.Middlewares.Auth;
using Newtonsoft.Json.Serialization;
using DataReef.Core.Infrastructure.Security;
using Newtonsoft.Json.Converters;

namespace DataReef.TM.ClientApi
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API authentication
            config.SuppressDefaultHostAuthentication(); // Remove other authentication flows
            config.Filters.Add(new HostAuthenticationFilter(TokenAuthOptions.CustomTokenAuthentication)); // We register the custom type of authentication
            config.Filters.Add(new AuthorizeAttribute()); // All apis are required to be authenticated by default 

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            config.Formatters.JsonFormatter.UseDataContractJsonSerializer = false;

            config.Formatters.JsonFormatter.SerializerSettings.Converters.Add(new StringEnumConverter());
        }
    }
}
