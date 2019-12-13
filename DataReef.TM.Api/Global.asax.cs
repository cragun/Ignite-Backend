using DataReef.Core.Common;
using DataReef.Core.Infrastructure.Bootstrap;
using DataReef.Core.Infrastructure.Unity;
using DataReef.Integrations.Spruce.DTOs;
using DataReef.TM.Api.Bootstrap;
using DataReef.TM.Contracts.Services;
using DataReef.TM.Models;
using DataReef.TM.Models.Commerce;
using DataReef.TM.Models.Credit;
using DataReef.TM.Models.Finance;
using DataReef.TM.Models.Layers;
using DataReef.TM.Models.PropertyAttachments;
using DataReef.TM.Models.PushNotifications;
using DataReef.TM.Models.Solar;
using DataReef.TM.Services;
using DataReef.TM.Services.Services;
using DataReef.TM.Services.Services.PropertyAttachments;
using Microsoft.AspNet.SignalR;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace DataReef.TM.Api
{
    /// <summary>
    /// HttpApplication custom implementation. The entry point for the API startup
    /// </summary>
    public class WebApiApplication : HttpApplication
    {
        protected void Application_Start()
        {
            // Bootstrap first
            Start();

            // Setup second

            var hubConfiguration = new HubConfiguration
            {
                EnableCrossDomain = true
            };

            AreaRegistration.RegisterAllAreas();
            RouteTable.Routes.MapHubs(hubConfiguration);
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap = new Dictionary<string, string>();

            System.Net.ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

            //GlobalConfiguration.Configuration.MessageHandlers.Add(new ApiLogHandler());

            // Need to explicitly load SqlServerTypes native libraries
            //try
            //{
            //    string binDir = Server.MapPath("~/bin");
            //    SqlServerTypes.Utilities.LoadNativeAssemblies(binDir);
            //}
            //catch { }
        }

        private static void Start()
        {
            //Create and register the global Unity container
            var container = new UnityContainer();
            GlobalConfiguration.Configuration.DependencyResolver = new UnityBootstrap(container);

            //Create and init infrastructure web api
            var appBootstrap = new WebApiBootstrap(container,
                AssemblyLoader.LoadAssemblies("bin", "DataReef.*.*.dll").ToArray(),
                new[]
                {
                    "DataReef.TM.Contracts.Services"
                });
            //Debugger.Launch();
            appBootstrap.Init();

            // register generic services
            RegisterDataServices(container);

            appBootstrap.RegisterServiceEnpoints();

            //Set MVC dependency resolver (for help controllers)
            DependencyResolver.SetResolver(new UnityDependencyResolver(container));

            // Setup the ambient service locator
            IServiceLocator locator = new UnityServiceLocator(container);
            ServiceLocator.SetLocatorProvider(() => locator);

            //Remap our custom controller selector to handle generic controller requests
            GlobalConfiguration.Configuration.Services.Replace(typeof(IHttpControllerSelector), new GenericControllerSelector(appBootstrap.Assemblies, GlobalConfiguration.Configuration));
            GlobalConfiguration.Configuration.Services.Replace(typeof(IHttpControllerTypeResolver), new GenericControllerTypeResolver(appBootstrap.Assemblies));

            //initialize automapper for services
            DataReef.TM.Services.Bootstrap.InitAutoMapper();
        }

        private static void RegisterDataServices(UnityContainer container)
        {
            container.RegisterType(typeof(IDataService<>), typeof(DataService<>));

            container.RegisterType(typeof(IDataService<PropertyActionItem>), typeof(ActionItemService));
            container.RegisterType(typeof(IDataService<AdderItem>), typeof(AdderItemService));
            container.RegisterType(typeof(IDataService<Appointment>), typeof(AppointmentService));
            container.RegisterType(typeof(IDataService<AppSetting>), typeof(AppSettingService));
            container.RegisterType(typeof(IDataService<Assignment>), typeof(AssignmentService));
            container.RegisterType(typeof(IDataService<CRUDAudit>), typeof(CRUDAuditService));
            container.RegisterType(typeof(IDataService<CurrentLocation>), typeof(CurrentLocationService));
            container.RegisterType(typeof(IDataService<Device>), typeof(DeviceService));
            container.RegisterType(typeof(IDataService<EpcStatus>), typeof(EpcStatusService));
            container.RegisterType(typeof(IDataService<FinancePlanDefinition>), typeof(FinancePlanDefinitionService));
            container.RegisterType(typeof(IDataService<Inquiry>), typeof(InquiryService));
            container.RegisterType(typeof(IDataService<Layer>), typeof(LayerService));
            container.RegisterType(typeof(IDataService<MediaItem>), typeof(MediaItemService));
            container.RegisterType(typeof(IDataService<Order>), typeof(OrderService));
            container.RegisterType(typeof(IDataService<OUAssociation>), typeof(OUAssociationService));
            container.RegisterType(typeof(IDataService<OU>), typeof(OUService));
            container.RegisterType(typeof(IDataService<OUSetting>), typeof(OUSettingService));
            container.RegisterType(typeof(IDataService<OUShape>), typeof(OUShapeService));
            container.RegisterType(typeof(IDataService<PersonKPI>), typeof(PersonKPIService));
            container.RegisterType(typeof(IDataService<Person>), typeof(PersonService));
            container.RegisterType(typeof(IDataService<PersonSetting>), typeof(PersonSettingService));
            container.RegisterType(typeof(IDataService<PropertyPowerConsumption>), typeof(PropertyPowerConsumptionService));
            container.RegisterType(typeof(IDataService<Property>), typeof(Property));
            container.RegisterType(typeof(IDataService<ProposalIntegrationAudit>), typeof(ProposalIntegrationAuditService));
            container.RegisterType(typeof(IDataService<Proposal>), typeof(ProposalService));
            container.RegisterType(typeof(IDataService<PushSubscription>), typeof(PushSubscriptionService));
            container.RegisterType(typeof(IDataService<DataReef.TM.Models.Spruce.GenDocsRequest>), typeof(SpruceGenDocsRequestService));
            container.RegisterType(typeof(IDataService<DataReef.TM.Models.Spruce.QuoteRequest>), typeof(SpruceQuoteRequestService));
            container.RegisterType(typeof(IDataService<DataReef.TM.Models.Spruce.QuoteResponse>), typeof(SpruceQuoteResponseService));
            container.RegisterType(typeof(IDataService<Territory>), typeof(TerritoryService));
            container.RegisterType(typeof(IDataService<TerritoryShape>), typeof(TerritoryShapeService));
            container.RegisterType(typeof(IDataService<UserInvitation>), typeof(UserInvitationService));
            container.RegisterType(typeof(IDataService<ZipArea>), typeof(ZipAreaService));
            container.RegisterType(typeof(IDataService<PrescreenBatch>), typeof(PrescreenBatchService));
            container.RegisterType(typeof(IDataService<PrescreenInstant>), typeof(PrescreenInstantService));
            container.RegisterType(typeof(IDataService<PropertyAttachment>), typeof(PropertyAttachmentService));
            container.RegisterType(typeof(IDataService<PropertyNote>), typeof(PropertyNoteService));
        }
    }
}