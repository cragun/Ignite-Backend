using System.Configuration;
using System.ServiceModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DataReef.Core.Attributes;

namespace DataReef.Core.Infrastructure.ServiceModel
{
    public class ProxyCatalog
    {
        private readonly Assembly[] assemblies;
        private readonly string[] proxyNamespaces;

        private readonly Proxy[] proxies;

        public ProxyCatalog(Assembly[] assemblies, string[] proxyNamespaces)
        {
            this.assemblies = assemblies;
            this.proxyNamespaces = proxyNamespaces;

            this.proxies = ScanServiceContracts();
        }

        public Proxy[] Services
        {
            get
            {
                return this.proxies;
            }
        }

        private Proxy[] ScanServiceContracts()
        {
            var services = new List<Proxy>();

            var contracts = this.assemblies.SelectMany(asm => asm.GetTypes()).Where(t => t.IsInterface && Attribute.IsDefined(t, typeof(ServiceContractAttribute)));

            foreach (var contractType in contracts)
            {
                if (!this.proxyNamespaces.Contains(contractType.Namespace))
                    continue;

                var baseAddress = ConfigurationManager.AppSettings[contractType.Namespace + ".BaseAddress"];

                if (string.IsNullOrEmpty(baseAddress))
                    throw new ConfigurationErrorsException("Missing " + contractType.Namespace + ".BaseAddress from conifguration file");

                if (!contractType.IsGenericType)
                {
                    services.Add(new Proxy
                    {
                        ServiceContract = contractType,
                        ServiceName = contractType.Name + ".svc",
                        BaseAddress = baseAddress
                    });
                }
                else
                {
                    //get any T constraints from IDataService<T> 
                    var constraints = contractType.GetGenericArguments()[0].GetGenericParameterConstraints();

                    if (constraints.Count() != 1) //todo: handle multiple constraints
                        throw new ArgumentException("The generic interface " + contractType + "defines unhandled contraints.");

                    var genericArgument = contractType.GetGenericArguments()[0].GetGenericParameterConstraints()[0];
                    var genericTypes = assemblies.SelectMany(a => a.GetTypes()).Where(t => t.IsSubclassOf(genericArgument));

                    foreach (var genericType in genericTypes)
                    {

                        var att = genericType.GetCustomAttributes<BootstrapExcludedAttribute>(false).FirstOrDefault();
                        if (att!=null && att.BootstrapType == BootstrapType.Api) continue;

                        services.Add(new Proxy
                        {
                            ServiceContract = contractType.MakeGenericType(genericType),
                            GenericBaseType = genericArgument,
                            ServiceName = genericType.Name + "Service.svc",
                            BaseAddress = baseAddress,
                        });
                    }
                }
            }

            return services.ToArray();
        }
    }
}
