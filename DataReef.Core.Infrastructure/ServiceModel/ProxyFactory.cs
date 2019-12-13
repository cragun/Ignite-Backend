using Microsoft.Practices.Unity;
using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;

namespace DataReef.Core.Infrastructure.ServiceModel
{
    public class ProxyFactory : InjectionMember
    {
        private readonly Proxy proxy;
        private static readonly ConcurrentDictionary<Type, Func<IUnityContainer, Object>> compiledfactoryMethods = new ConcurrentDictionary<Type, Func<IUnityContainer, object>>();
        private readonly InjectionFactory injectionFactory;
      
        public ProxyFactory(Proxy proxy)
        {
            this.injectionFactory = new InjectionFactory(this.ProxyFactoryMethod);
            this.proxy = proxy;
        }

        private object ProxyFactoryMethod(IUnityContainer container, Type serviceType, string contractName)
        {
            var compiledMethod = compiledfactoryMethods.GetOrAdd(serviceType, this.CreateCompiledFactoryFunction);
            return compiledMethod(container);
        }

        public Func<IUnityContainer, Object> CreateCompiledFactoryFunction(Type newType)
        {
            Expression<Func<ProxyChannelFactory, object>> mehtodExpression = c => c.CreateChannel<object>();
            var mc = (MethodCallExpression)mehtodExpression.Body;
            MethodInfo newMethod = mc.Method.GetGenericMethodDefinition().MakeGenericMethod(newType);

            var proxyChannel = Expression.Parameter(typeof(ProxyChannelFactory), "c");

            var func = (Func<ProxyChannelFactory, object>)Expression.Lambda(Expression.Call(proxyChannel, newMethod), proxyChannel).Compile();

            return container =>
            {
                var proxyChannelFactory = container.Resolve<ProxyChannelFactory>(new ParameterOverride("serviceBaseAddress", this.proxy.BaseAddress));
                return func(proxyChannelFactory);
            };
        }


        public override void AddPolicies(Type serviceType, Type implementationType, string name, Microsoft.Practices.ObjectBuilder2.IPolicyList policies)
        {
            this.injectionFactory.AddPolicies(serviceType, implementationType, name, policies);
        }
    }

}