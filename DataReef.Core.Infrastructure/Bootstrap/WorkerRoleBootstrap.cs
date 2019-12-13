using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using DataReef.Core.Common;
using DataReef.Core.Infrastructure.ServiceModel;

namespace DataReef.Core.Infrastructure.Bootstrap
{
    public class WorkerRoleBootstrap
    {
        private readonly Assembly[] assemblies;

        public string[] ProxyNamespaces { get; set; }

        private readonly UnityContainer container;

        public WorkerRoleBootstrap()
        {            
            this.container = new UnityContainer();
            this.assemblies = AssemblyLoader.LoadAssemblies("DataReef.*.*.dll").ToArray();
            Trace.TraceInformation("WorkerRoleBootstrap: found " + assemblies.Count() + " assemblies");
        }

        public void Run()
        {
            //Register Unity
            this.container.RegisterInstance<IServiceLocator>(new UnityServiceLocator(this.container));
            Trace.TraceInformation("WorkerRoleBootstrap: UnityServiceLocator registered ");

            //Set the ambient ServiceLocator to point to he UnityServiceLocator=>UnityContainer
            ServiceLocator.SetLocatorProvider(() => container.Resolve<IServiceLocator>());
            Trace.TraceInformation("WorkerRoleBootstrap: ServiceLocator registered ");

            //Register in-memory services
            RegisterInMemoryServices();
            Trace.TraceInformation("WorkerRoleBootstrap: RegisterInMemoryServices done!");

            //Register proxies to external services
            RegisterProxies();
            Trace.TraceInformation("WorkerRoleBootstrap: RegisterProxies done.");
        }

        private void RegisterInMemoryServices()
        {
            //Register application services
            var applicationServicesBootstrapper = new ApplicationServicesBootstrapper(assemblies, this.container);
            applicationServicesBootstrapper.RegisterApplicationServices();
        }

        private void RegisterProxies()
        {
            var proxyCatalog = new ProxyCatalog(this.assemblies, ProxyNamespaces);

            foreach (var proxyType in proxyCatalog.Services)
            {
                if (!container.IsRegistered(proxyType.ServiceContract))
                    container.RegisterType(proxyType.ServiceContract, new ProxyFactory(proxyType));
            }
        }
    }
}
