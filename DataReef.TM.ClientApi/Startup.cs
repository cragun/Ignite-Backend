using DataReef.Core.Infrastructure.Bootstrap;
using DataReef.Core.Infrastructure.Unity;
using DataReef.TM.ClientApi.Middlewares.Auth;
using Microsoft.Owin;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using Owin;
using System.Web.Http;
using Swashbuckle.Application;
using System.IO;
using DataReef.Core.Common;
using System.Linq;

[assembly: OwinStartup(typeof(DataReef.TM.ClientApi.Startup))]
namespace DataReef.TM.ClientApi
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // Create and register the global Unity container
            var container = new UnityContainer();

            // Bootstrap, build the contrainer
            var appBootstrap = new WebApiBootstrap(container,
                AssemblyLoader.LoadAssemblies("bin", "DataReef.*.*.dll").ToArray(),
                new[]
                {
                    "DataReef.TM.Contracts.Services"
                });
            appBootstrap.Init();

            // Setup WebAPI
            var config = new HttpConfiguration();
            config.DependencyResolver = new UnityBootstrap(container);
            WebApiConfig.Register(config);

            config.EnableSwagger(c =>
                {
                    c.SingleApiVersion("v1", "DataReef.TM.ClientApi");
                    c.PrettyPrint();
                    c.IncludeXmlComments($@"{System.AppDomain.CurrentDomain.BaseDirectory}\bin\DataReef.TM.ClientApi.XML");
                    // try to get the Models xml file from the \bin folder (dev environment)
                    var modelsXmlFilePath = $@"{System.AppDomain.CurrentDomain.BaseDirectory}\bin\DataReef.TM.Models.xml";

                    if (File.Exists(modelsXmlFilePath))
                    {
                        c.IncludeXmlComments(modelsXmlFilePath);
                    }
                    else
                    {
                        // if it can't find it there (when deploying, it will be in the root folder)
                        modelsXmlFilePath = $@"{System.AppDomain.CurrentDomain.BaseDirectory}\DataReef.TM.Models.xml";
                        if (File.Exists(modelsXmlFilePath))
                        {
                            c.IncludeXmlComments(modelsXmlFilePath);
                        }
                    }

                    c.DescribeAllEnumsAsStrings();
                })
                .EnableSwaggerUi(c =>
                {
                    c.DocumentTitle("DataReef Gateway");
                    c.DisableValidator();
                });

            // Setup the ambient service locator
            IServiceLocator locator = new UnityServiceLocator(container);
            ServiceLocator.SetLocatorProvider(() => locator);

            // Final registration of pipeline, the order is important!
            app.UseTokenAuthentication();
            app.UseWebApi(config);
        }
    }
}