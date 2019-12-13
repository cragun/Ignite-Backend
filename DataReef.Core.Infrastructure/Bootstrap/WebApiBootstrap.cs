using System.Linq;
using System.Reflection;
using Microsoft.Practices.Unity;
using DataReef.Core.Common;
using DataReef.Core.Infrastructure.ServiceModel;
using System.Collections.Generic;
using System.ServiceModel;
using System;

namespace DataReef.Core.Infrastructure.Bootstrap
{
    //todo: both bootstraps shoud inherit from base that should cover (in-memory services and proxies)
    public class WebApiBootstrap
    {
        private readonly string[] proxyNamespaces;
        private readonly string[] serviceContractNamespaces;

        private readonly IUnityContainer container;

        public WebApiBootstrap(IUnityContainer container, Assembly[] assemblies, string[] serviceContractNamespaces)
        {
            this.Assemblies = assemblies;
            this.container = container;
            this.serviceContractNamespaces = serviceContractNamespaces;
        }

        public Assembly[] Assemblies { get; private set; }

        public void Init()
        {
            //Register application in-mem services (e.g. [Service])
            var applicationServicesBootstrapper = new ApplicationServicesBootstrapper(Assemblies, this.container);
            applicationServicesBootstrapper.RegisterApplicationServices();
        }

        /// <summary>
        /// Scan all WCF services to be exposed as endpoints e.g. [WcfService(typeof(IMyserviceContract))]
        /// </summary>
        public void RegisterServiceEnpoints()
        {
            var registeredServices = ScanServiceContracts();
            foreach (var regService in registeredServices)
            {
                if (!container.IsRegistered(regService.ServiceContract))
                {
                    this.container.RegisterType(regService.ServiceContract, regService.ServiceType);
                }
            }
        }

        private IEnumerable<ServiceRegistration> ScanServiceContracts()
        {
            var services = new List<ServiceRegistration>();
            var types = this.Assemblies
                .SelectMany(asm => asm.GetTypes())
                .Where(t => t.IsInterface 
                            && t.Namespace == "DataReef.TM.Contracts.Services"
                            && Attribute.IsDefined(t, typeof(ServiceContractAttribute)));

            foreach (var type in types)
            {
                if (!type.IsGenericType)
                {
                    var serviceImplementation = this.Assemblies.SelectMany(asm => asm.GetTypes())
                        .FirstOrDefault(t => t.Namespace != null && (t.IsClass && !t.Namespace.Contains("Test")
                                                                     && type.IsAssignableFrom(t)));
                    if (serviceImplementation == null)
                        continue;

                    if (FilterServiceInheritence(services, type))
                        continue;

                    services.Add(new ServiceRegistration
                    {
                        ServiceType = serviceImplementation,
                        ServiceContract = type,
                    });
                }
                else
                {
                    var serviceImplementation = this.Assemblies.SelectMany(asm => asm.GetTypes())
                        .FirstOrDefault(t => t.IsClass && t.IsGenericType &&
                               t.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == type));

                    if (serviceImplementation == null)
                        continue;

                    var genericArgument = serviceImplementation.GetGenericArguments()[0].GetGenericParameterConstraints()[0];

                    var genericTypes = Assemblies.SelectMany(a => a.GetTypes()).Where(t => t.IsSubclassOf(genericArgument));

                    foreach (var genericType in genericTypes)
                    {
                        var makeGenericType = type.MakeGenericType(genericType);

                        if (FilterServiceInheritence(services, makeGenericType))
                            continue;

                        services.Add(new ServiceRegistration
                        {
                            ServiceType = serviceImplementation.MakeGenericType(genericType),
                            ServiceContract = makeGenericType,
                            GenericBaseType = genericArgument
                        });
                    }
                }
            }

            return services.ToArray();
        }

        /// <summary>
        /// If the service interface is already registers with a subtype return true. No need to register a new service entry.
        /// If there are other supertype remove them from the <param name="registeredServices"></param> list.
        /// </summary>
        private static bool FilterServiceInheritence(List<ServiceRegistration> registeredServices, Type interfaceType)
        {
            Type[] implementedInterfaces = interfaceType.GetInterfaces();

            foreach (var service in registeredServices.ToList())
            {
                // the service is already registered with a subtype
                if (interfaceType.IsAssignableFrom(service.ServiceContract))
                    return true;

                // if this new interfaceType implements an existing service remove that service
                if (implementedInterfaces.Any(x => x == service.ServiceContract))
                    registeredServices.Remove(service);
            }

            return false;
        }
    }
}
