using DataReef.Core.Infrastructure.ServiceModel.Bindings;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;

namespace DataReef.Core.Infrastructure.ServiceModel
{
    public class ProxyChannelFactory
    {
        private readonly string serviceBaseAddress;

        public ProxyChannelFactory(string serviceBaseAddress)
        {
            this.serviceBaseAddress = serviceBaseAddress;
        }

        public T CreateChannel<T>()
        {
            Type serviceType = typeof(T);

            var factory = new ChannelFactory<T>(CreateBinaryHttpBinding(), serviceBaseAddress + GetAddress(serviceType));

            this.AddBehaviors(factory);

            return factory.CreateChannel();
        }

        private void AddBehaviors<T>(ChannelFactory<T> factory)
        {
            var endpointBehaviours = ServiceLocator.Current.GetAllInstances<IEndpointBehavior>().ToList();
            foreach (var endpointBehaviour in endpointBehaviours)
            {
                factory.Endpoint.Behaviors.Add(endpointBehaviour);
            }
        }

        public Binding CreateBinaryHttpBinding()
        {
            var binaryHttpBinding = new BinaryHttpBinding
            {
                SendTimeout = new TimeSpan(0, 0, 5, 0),
            };

            return binaryHttpBinding;
        }

        //TODO: this is currently a convention . Move to some attribute
        public string GetAddress(Type serviceType)
        {
            if (serviceType.IsGenericType)
            {
                var entityType = serviceType.GenericTypeArguments[0];
                return entityType.Name + "Service.svc";
            }
            if (serviceType.IsInterface)
                return serviceType.Name.Substring(1) + ".svc";

            return serviceType.Name + ".svc";
        }
    }
}