using System;
using System.Linq;
using System.Reflection;
using Microsoft.Practices.Unity;
using DataReef.Core.Attributes;
using System.Collections.Generic;
using DataReef.Core.Infrastructure.Unity;

namespace DataReef.Core.Infrastructure.Bootstrap
{
    //todo: revisit this and find a better namespace for it
    public class ApplicationServicesBootstrapper
    {
        private readonly Assembly[] assemblies;
        private readonly IUnityContainer container;

        public ApplicationServicesBootstrapper(Assembly[] assemblies, IUnityContainer container)
        {
            this.assemblies = assemblies;
            this.container = container;
        }

        public int RegisterApplicationServices()
        {
            var count = 0;

            var tt = this.assemblies.SelectMany(asm => asm.GetTypes())
                .Where(t => t.IsClass && Attribute.IsDefined(t, typeof(ServiceAttribute)));


            foreach (var type in tt)
            {
                //One concrete class can implement more than one service (not desirable, but possible)
                foreach (var serviceAttribute in type.GetCustomAttributes<ServiceAttribute>())
                {
                    this.container.RegisterType(serviceAttribute.ServiceType, type, serviceAttribute.ServiceName, CreateLifetimeManager(serviceAttribute), new InjectionMember[] { });
                    count++;
                }
            }
            return count;
        }

        private static readonly Dictionary<ServiceScope, Func<string, LifetimeManager>> LifetimeManagers =
                new Dictionary<ServiceScope, Func<string, LifetimeManager>>
                                   {
                                       {
                                           ServiceScope.None, serviceName => new PerResolveLifetimeManager()
                                       },
                                       {
                                           ServiceScope.Application, serviceName => new ContainerControlledLifetimeManager()
                                       },
                                       {
                                           ServiceScope.Request, serviceName => new RequestLifetimeManager(serviceName)},
                                       {
                                           ServiceScope.Session, serviceName => new SessionLifetimeManager(serviceName)
                                       }
                                   };

        private static LifetimeManager CreateLifetimeManager(ServiceAttribute serviceAttribute)// (ServiceScope regularService, Type type, string serviceName)
        {
            var serviceScope = serviceAttribute.Scope;
            var serviceType = serviceAttribute.ServiceType;
            var serviceName = !string.IsNullOrEmpty(serviceAttribute.ServiceName) ? serviceAttribute.ServiceName : serviceType.FullName;

            var factory = LifetimeManagers[serviceScope];

            var lifetimeManager = factory(serviceName);

            return lifetimeManager;
        }
    }
}
