using System.Diagnostics;
using System.ServiceModel;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using DataReef.Core.Common;
using DataReef.Core.Infrastructure.ServiceModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace DataReef.Core.Infrastructure.Bootstrap
{
    public class WcfBootstrap
    {
        private readonly string[] serviceContractNamespaces;
        private readonly string[] proxyNamespaces;
        private readonly Assembly[] assemblies;

        private readonly UnityContainer container;

        public WcfBootstrap(string[] serviceContractNamespaces, string[] proxyNamespaces)
        {
            this.serviceContractNamespaces = serviceContractNamespaces;
            this.proxyNamespaces = proxyNamespaces;
            this.container = new UnityContainer();
            this.assemblies = AssemblyLoader.LoadAssemblies("DataReef.*.*.dll").ToArray();

            Trace.TraceInformation("WcfBootstrap: Constructor found " + assemblies.Count() + " assemblies.");
        }

        public void Run()
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            try
            {
                Trace.TraceInformation("WcfBootstrap: Started.");
                WcfServiceRegistrator.EnsureInitialized();

                //Register Unity
                this.container.RegisterInstance<IServiceLocator>(new UnityServiceLocator(this.container));
                Trace.TraceInformation("WcfBootstrap: UnityServiceLocator registered.");

                //Set the ambient ServiceLocator to point to he UnityServiceLocator=>UnityContainer
                ServiceLocator.SetLocatorProvider(() => container.Resolve<IServiceLocator>());

                Trace.TraceInformation("WcfBootstrap: UnityServiceLocator set as the ambient ServiceLocator.");

                //Register in-memory services
                var inMemoryServices = RegisterInMemoryServices();
                Trace.TraceInformation("WcfBootstrap: Registered " + inMemoryServices + " in-memory services");

                //Register service endpoints
                var serviceEndpoints = RegisterServiceEnpoints();
                Trace.TraceInformation("WcfBootstrap: Registered " + serviceEndpoints + " service endpoints.");

                //Register proxies to external services
                var proxyFactories = RegisterProxies();
                Trace.TraceInformation("WcfBootstrap: Registered " + proxyFactories + " proxy factories.");

                stopWatch.Stop();
                Trace.TraceInformation("WcfBootstrap: Finished successfully in " + stopWatch.ElapsedMilliseconds + " milliseconds.");
            }
            catch (Exception ex)
            {
                Trace.TraceError("WcfBootstrap ERROR");
                Trace.TraceError(ex.Message, new object[] { ex.StackTrace, ex.InnerException });
            }
        }

        private int RegisterProxies()
        {
            var count = 0;
            var proxyCatalog = new ProxyCatalog(this.assemblies, this.proxyNamespaces);

            foreach (var proxyType in proxyCatalog.Services)
            {
                if (!container.IsRegistered(proxyType.ServiceContract))
                {
                    container.RegisterType(proxyType.ServiceContract, new ProxyFactory(proxyType));
                    count++;
                }
            }
            return count;
        }

        private int RegisterInMemoryServices()
        {
            //Register application services
            var applicationServicesBootstrapper = new ApplicationServicesBootstrapper(assemblies, this.container);
            return applicationServicesBootstrapper.RegisterApplicationServices();
        }

        /// <summary>
        /// Scan all WCF services to be exposed as endpoints e.g. [WcfService(typeof(IMyserviceContract))]
        /// </summary>
        private int RegisterServiceEnpoints()
        {
            var count = 0;
            var wcfServices = ScanServiceContracts();
            foreach (var wcfService in wcfServices)
            {
                if (!container.IsRegistered(wcfService.ServiceContract))
                {
                    this.container.RegisterType(wcfService.ServiceContract, wcfService.ServiceType);
                }

                //register as WCF service
                var address = string.IsNullOrEmpty(wcfService.Address) ? wcfService.ServiceType.Name + ".svc" : wcfService.Address;
                WcfServiceRegistrator.RegisterService(String.Format(CultureInfo.InvariantCulture, "~/{0}", address), typeof(SmartServiceHostFactory), wcfService.ServiceType);
                count++;
            }
            return count;
        }

        private IEnumerable<WcfService> ScanServiceContracts()
        {
            var services = new List<WcfService>();
            var types = this.assemblies.SelectMany(asm => asm.GetTypes()).Where(t => t.IsInterface && Attribute.IsDefined(t, typeof(ServiceContractAttribute)));

            foreach (var type in types)
            {
                if (!serviceContractNamespaces.Contains(type.Namespace))
                {
                    continue;
                }

                if (!type.IsGenericType)
                {
                    var serviceImplementation = this.assemblies.SelectMany(asm => asm.GetTypes())
                        .FirstOrDefault(t => t.Namespace != null && (t.IsClass && !t.Namespace.Contains("Test")
                                                                     && type.IsAssignableFrom(t)));
                    if (serviceImplementation == null)
                        continue;

                    if (FilterServiceInheritence(services, type))
                        continue;

                    services.Add(new WcfService
                    {
                        ServiceType = serviceImplementation,
                        ServiceContract = type,
                    });
                }
                else
                {
                    var serviceImplementation = this.assemblies.SelectMany(asm => asm.GetTypes())
                        .FirstOrDefault(t => t.IsClass && t.IsGenericType &&
                               t.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == type));

                    if (serviceImplementation == null)
                        continue;

                    var genericArgument = serviceImplementation.GetGenericArguments()[0].GetGenericParameterConstraints()[0];

                    var genericTypes = assemblies.SelectMany(a => a.GetTypes()).Where(t => t.IsSubclassOf(genericArgument));

                    foreach (var genericType in genericTypes)
                    {
                        var makeGenericType = type.MakeGenericType(genericType);

                        if (FilterServiceInheritence(services, makeGenericType))
                            continue;

                        services.Add(new WcfService
                        {
                            ServiceType = serviceImplementation.MakeGenericType(genericType),
                            ServiceContract = makeGenericType,
                            GenericBaseType = genericArgument,
                            Address = genericType.Name + "Service.svc"
                        });
                    }
                }
            }

            return services.ToArray();
        }

        /// <summary>
        /// If the service interface is already registers with a subtype return true. No need to register a new service entry.
        /// If there are other supertype remove them from the <param name="wcfServices"></param> list.
        /// </summary>
        private static bool FilterServiceInheritence(List<WcfService> wcfServices, Type interfaceType)
        {
            Type[] implementedInterfaces = interfaceType.GetInterfaces();

            foreach (var service in wcfServices.ToList())
            {
                // the service is already registered with a subtype
                if (interfaceType.IsAssignableFrom(service.ServiceContract))
                    return true;

                // if this new interfaceType implements an existing service remove that service
                if (implementedInterfaces.Any(x => x == service.ServiceContract))
                    wcfServices.Remove(service);
            }

            return false;
        }
    }
}
