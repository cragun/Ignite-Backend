using Microsoft.Practices.ServiceLocation;
using System;
using System.ServiceModel;
using System.ServiceModel.Dispatcher;

namespace DataReef.Core.Infrastructure.ServiceModel
{
    public class InstanceProvider : IInstanceProvider
    {
        private readonly Type serviceType;

        public InstanceProvider(Type serviceType)
        {
            this.serviceType = serviceType;
        }

        /// <summary>
        /// This method is the Hook from the WCF - UnityContainer.
        /// Here the actual service instance will be created by Unity
        /// </summary>
        /// <param name="instanceContext"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public object GetInstance(InstanceContext instanceContext, System.ServiceModel.Channels.Message message)
        {
            object instance = ServiceLocator.Current.GetInstance(this.serviceType);
            return instance;
        }

        public object GetInstance(InstanceContext instanceContext)
        {
            return GetInstance(instanceContext, null);
        }

        public void ReleaseInstance(InstanceContext instanceContext, object instance)
        {
            var disposable = instance as IDisposable;
            if (disposable != null)
                disposable.Dispose();
        }
    }
}
