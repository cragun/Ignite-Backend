using DataReef.Core.Infrastructure.Authorization;
using DataReef.Core.Infrastructure.ServiceModel.Bindings;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Policy;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;

namespace DataReef.Core.Infrastructure.ServiceModel
{
    /// <summary>
    /// Custom service host that does the following things:
    /// 1. Add a DependencyInjectionServiceBehavior => hook the instance creation to an DI container
    /// 2. Add a ServiceAuthorizationBehaviour => hook authorization checks 
    /// </summary>
    public class SmartServiceHost : ServiceHost
    {
        public SmartServiceHost(Type serviceType, params Uri[] baseAddresses)
            : base(serviceType, baseAddresses)
        {
            base.ApplyConfiguration();

            var contractInterfaces = serviceType.GetInterfaces().Where(i => i.GetCustomAttributes(typeof(ServiceContractAttribute), false).Any()).ToList();
            var mostCompleteContractInterface = GetMostImportantContractInteface(contractInterfaces);
            AddServiceEndpoint(mostCompleteContractInterface, CreateBinaryHttpBinding(), "");

            RegisterServiceBehaviors(Description);
        }

        /// <summary>
        /// TODO: We only register the most important type of endpoint for a service atm. Most important is the inteface that implements the most intefaces.
        /// This needs to change. A way to tag a interface that should get there own endpoint
        /// </summary>
        private static Type GetMostImportantContractInteface(ICollection<Type> contractInterfaces)
        {
            var interfaceDictionary = new Dictionary<Type, int>();

            foreach (var contractInterface in contractInterfaces)
            {
                var otherIntefacesImplemented = contractInterface.GetInterfaces();
                var implementsInterfaces = 0;

                foreach (var otherInterfaces in contractInterfaces)
                {
                    if (otherInterfaces == contractInterface)
                        continue;

                    if (otherIntefacesImplemented.Any(i => i == otherInterfaces))
                        implementsInterfaces++;
                }

                interfaceDictionary.Add(contractInterface, implementsInterfaces);
            }

            // pick the interface with the most inherited types
            return interfaceDictionary.OrderByDescending(r => r.Value).First().Key;
        }

        private static void RegisterServiceBehaviors(ServiceDescription serviceDescription)
        {
            var serviceLocator = ServiceLocator.Current;
            var serviceBehaviors = serviceLocator.GetAllInstances<IServiceBehavior>().ToList();
            var endpointBehaviors = serviceLocator.GetAllInstances<IEndpointBehavior>().ToList();
            var contractBehaviors = serviceLocator.GetAllInstances<IContractBehavior>().ToList();
            var operationBehaviors = serviceLocator.GetAllInstances<IOperationBehavior>().ToList();
            var authBehaviors = serviceLocator.GetAllInstances<IAuthorizationPolicy>().ToList();

            AddBehaviors(serviceDescription.Behaviors, serviceBehaviors);

            foreach (var endpoint in serviceDescription.Endpoints)
            {
                AddBehaviors(endpoint.Behaviors, endpointBehaviors);
                AddBehaviors(endpoint.Contract.Behaviors, contractBehaviors);

                foreach (var operation in endpoint.Contract.Operations)
                    AddBehaviors(operation.Behaviors, operationBehaviors);
            }

            if (ApplyAuthorizationBehaviour())
            {
                var serviceAuthorization = serviceDescription.Behaviors.Find<ServiceAuthorizationBehavior>();
                if (serviceAuthorization == null)
                {
                    serviceAuthorization = new ServiceAuthorizationBehavior();
                    serviceDescription.Behaviors.Add(serviceAuthorization);
                }
                serviceAuthorization.ServiceAuthorizationManager = new CustomAuthorizationManager();
                serviceAuthorization.PrincipalPermissionMode = PrincipalPermissionMode.None;
            }
        }

        protected override void OnOpen(TimeSpan timeout)
        {
            Description.Behaviors.Add(new DependencyInjectionServiceBehavior());

            var mex = Description.Behaviors.Find<ServiceMetadataBehavior>();
            if (mex == null)
            {
                mex = new ServiceMetadataBehavior();
                Description.Behaviors.Add(mex);
            }
            mex.HttpGetEnabled = true;

#if DEBUG
            var debugBehaviour = Description.Behaviors.Find<ServiceDebugBehavior>();
            debugBehaviour.IncludeExceptionDetailInFaults = true;
#endif
            base.OnOpen(timeout);
        }

        public static bool ApplyAuthorizationBehaviour()
        {
            var applyAuthBehaviour = ConfigurationManager.AppSettings["ApplyAuthorizationBehaviour"];

            bool ret;

            if (bool.TryParse(applyAuthBehaviour, out ret))
                return ret;

            return true;
        }

        private static void AddBehaviors<T>(ICollection<T> collection, IEnumerable<T> behaviors)
        {
            foreach (var behavior in behaviors.Where(behavior => !collection.Contains(behavior)))
                collection.Add(behavior);
        }

        public Binding CreateBinaryHttpBinding()
        {
            var binaryHttpBinding = new BinaryHttpBinding
            {
                SendTimeout = new TimeSpan(0, 0, 5, 0),
            };

            return binaryHttpBinding;
        }
    }

}
