using System;
using System.Web;
using DataReef.Core.Infrastructure.Bootstrap;

[assembly: PreApplicationStartMethod(typeof(DataReef.SyncEngine.IISHost.ServicesInit), "Init")]

namespace DataReef.SyncEngine.IISHost
{
    public static class ServicesInit
    {
        public static void Init()
        {
            try
            {
                var applicationBoostrap = new WcfBootstrap(
                    new string[]
                    {
                        "DataReef.Sync.Contracts"
                    },
                    new string[]
                    {
                        "DataReef.TM.Contracts.Services"
                    });

                applicationBoostrap.Run();

                System.Diagnostics.Trace.TraceInformation("ServicesInit Complete");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError("ServicesInit Failed");
                System.Diagnostics.Trace.TraceError(ex.Message, new object[] { ex.StackTrace, ex.InnerException });
            }
        }
    }
}