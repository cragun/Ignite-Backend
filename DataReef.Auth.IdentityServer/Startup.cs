using System;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Configuration;
using System.Web.Http;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Net.Http.Formatting;
using System.Threading;
using System.Web.Routing;
using System.ServiceModel.Activation;
using Owin;
using Microsoft.Owin;
using Thinktecture.IdentityServer.Core.Configuration;
using Thinktecture.IdentityServer.Core.Logging;
using DataReef.Auth.IdentityServer.DataAccess;
using DataReef.Auth.IdentityServer.Helpers;
using DataReef.Auth.IdentityServer.Config;
using DataReef.Auth.IdentityServer.Endpoints;

namespace DataReef.Auth.IdentityServer
{
    public class Startup
    {
        public void Configuration(IAppBuilder appBuilder)
        {
            LogProvider.SetCurrentLogProvider(new NLogLogProvider());

            // Web API host configuration 
            SetupWebApiServer(appBuilder);

            // Identity server setup
            SetupIdentityServer(appBuilder);

            // WCF services setup
            SetupWcfServer();
        }

        private void SetupWebApiServer(IAppBuilder appBuilder)
        {
            HttpConfiguration config = new HttpConfiguration();
            config.Formatters.Remove(config.Formatters.FormUrlEncodedFormatter);
            config.Formatters.Remove(config.Formatters.XmlFormatter);
            config.Formatters.Add(new JsonMediaTypeFormatter());
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "admin/{action}/{id}",
                defaults: new { controller = "useradministration", id = RouteParameter.Optional }
            );
            appBuilder.UseWebApi(config);
        }

        private void SetupIdentityServer(IAppBuilder appBuilder)
        {
            // Detect JSON requests for the token endpoint and transform it to form url encoded
            appBuilder.Use<JsonTokenClientDetector>();

            // Intercept failed authentications and update the http status code
            appBuilder.Use<AuthenticationResponseProcessor>();

            IdentityServerOptions options = new IdentityServerOptions
            {
                IssuerUri = ConfigurationManager.AppSettings["TokenIssuerUri"],
                SiteName = "SmartCare IdentityServer",
                RequireSsl = false,
                SigningCertificate = Certificate.Get(),
                Factory = IdentityServiceFactory.Configure(),
                CorsPolicy = CorsPolicy.AllowAll
            };
            appBuilder.UseIdentityServer(options);
        }

        private void SetupWcfServer()
        {
            ServiceHostFactory factory = new ServiceHostFactory();
            RouteTable.Routes.Add(new ServiceRoute("useradministration", factory, typeof(UserAdministrationService)));
        }
    }
}
