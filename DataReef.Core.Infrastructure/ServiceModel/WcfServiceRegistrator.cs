using System;
using System.Collections;
using System.Configuration;
using System.Globalization;
using System.Reflection;
using System.ServiceModel;

namespace DataReef.Core.Infrastructure.ServiceModel
{
    public static class WcfServiceRegistrator
    {
        static readonly object _syncRoot = new object();
        private static FieldInfo hostingManagerField;
        private static MethodInfo ensureInitialized;
        static FieldInfo serviceActivationsField;

        public static void EnsureInitialized()
        {
            try
            {
                hostingManagerField = typeof(ServiceHostingEnvironment).GetField("hostingManager", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.GetField);
                ensureInitialized = typeof(ServiceHostingEnvironment).GetMethod("EnsureInitialized", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.InvokeMethod);
                ensureInitialized.Invoke(null, new object[] { });
            }
            catch (Exception ex)
            {
                throw new ApplicationException("EnsureInitialized() failed ", ex);
            }
        }

        public static void RegisterService(string addr, Type factory, string serviceFilePath)
        {
            lock (_syncRoot)
            {
                var serviceActivations = GetServceActivations();

                string value = string.Format(CultureInfo.InvariantCulture,"{0}|{1}|{2}", addr, factory.AssemblyQualifiedName, serviceFilePath);

                if (!serviceActivations.ContainsKey(addr))
                {
                    serviceActivations.Add(addr, value);
                }
            }
        }


        public static void RegisterService(string addr, Type factory, Type service)
        {
            lock (_syncRoot)
            {
                var serviceActivations = GetServceActivations();

                string value = string.Format(CultureInfo.InvariantCulture, "{0}|{1}|{2}", addr, factory.AssemblyQualifiedName, service.AssemblyQualifiedName);

                if (!serviceActivations.ContainsKey(addr))
                {
                    serviceActivations.Add(addr, value);
                }
            }
        }

        private static Hashtable GetServceActivations()
        {            
            // Hijack the WCF 4 Configuration Based Activation
            // Insert our own services in the serviceHostingEnvironment section            
            //      <serviceHostingEnvironment multipleSiteBindingsEnabled="true" >
            //          <serviceActivations>
            //              <add relativeAddress="PersonService.svc" service="DataReef.TM.PersonService" />
            //          </serviceActivations>
            //      </serviceHostingEnvironment>
            
            object hostingManager = hostingManagerField.GetValue(null);
            try
            {
                if (serviceActivationsField == null)
                {
                    serviceActivationsField = hostingManager.GetType().GetField("serviceActivations", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField);
                    if (serviceActivationsField == null)
                    {
                        throw new ConfigurationErrorsException("Invalid serviceActivation configuraiton section");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ConfigurationErrorsException("Invalid serviceActivation configuraiton section", ex);
            }

            var serviceActivations = (Hashtable)serviceActivationsField.GetValue(hostingManager);
            return serviceActivations;
        }
    }
}
