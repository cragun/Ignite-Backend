using System;
using System.ServiceModel;
using System.ServiceModel.Activation;

namespace DataReef.Core.Infrastructure.ServiceModel
{
    /// <summary>
    /// Custom ServiceHost factory that will create a SmartServiceHost
    /// </summary>
    public class SmartServiceHostFactory : ServiceHostFactory
    {
        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            var serviceHost = new SmartServiceHost(serviceType, baseAddresses);
            return serviceHost;
        }
    }
}
