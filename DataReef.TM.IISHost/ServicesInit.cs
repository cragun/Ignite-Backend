using DataReef.Core.Infrastructure.Bootstrap;
using DataReef.TM.Services;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Web;

[assembly: PreApplicationStartMethod(typeof(DataReef.TM.IISHost.ServicesInit), "Init")]

namespace DataReef.TM.IISHost
{
    /// <summary>
    /// This is the application entry point that will fire the WCF Boostrap for
    /// 1. Unity Container creation
    /// 2. WCF endpoints registration
    /// 3. WCF proxies registration to access other services 
    /// </summary>
    public static class ServicesInit
    {
        public static void Init()
        {
            // Need to explicitly load SqlServerTypes native libraries
            try
            {
                var binDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bin");
                SqlServerTypes.Utilities.LoadNativeAssemblies(binDir);
            }
            catch { }

            // Velocify endpoint has disabled TLS 1.0, that's why we need to enable TLS 1.1 and TLS 1.2
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

            try
            {
                var applicationBoostrap = new WcfBootstrap(
                    new[]
                    {
                        "DataReef.TM.Contracts.Services"
                    },
                    new string[] { });
                applicationBoostrap.Run();
            }
            catch (ReflectionTypeLoadException ex)
            {
                foreach (var loadEx in ex.LoaderExceptions)
                {
                    try
                    {
                        Recurse(loadEx);
                    }
                    catch
                    {
                    }
                }
            }
            catch (Exception ex)
            {
                try
                {
                    Recurse(ex);
                }
                catch
                {
                }
            }

            Bootstrap.InitAutoMapper();
        }

        private static void Recurse(Exception ex)
        {
            EventLog.WriteEntry("Application", ex.Message, EventLogEntryType.Error);
            EventLog.WriteEntry("Application", ex.StackTrace ?? string.Empty, EventLogEntryType.Error);
            if (ex.InnerException != null)
            {
                Recurse(ex.InnerException);
            }
        }
    }
}