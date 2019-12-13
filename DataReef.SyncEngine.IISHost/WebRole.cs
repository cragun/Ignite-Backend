using System;
using System.Diagnostics;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;

namespace DataReef.SyncEngine.IISHost
{
    public class WebRole : RoleEntryPoint
    {
        public override bool OnStart()
        {
            try
            {
                //To enable the AzureLocalStorageTraceListner, uncomment relevent section in the web.config  
                DiagnosticMonitorConfiguration diagnosticConfig = DiagnosticMonitor.GetDefaultInitialConfiguration();
                diagnosticConfig.Directories.ScheduledTransferPeriod = TimeSpan.FromMinutes(1);
                diagnosticConfig.Directories.DataSources.Add(AzureLocalStorageTraceListener.GetLogDirectory());

                // For information on handling configuration changes
                // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.

            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.Message, new object[] { ex.StackTrace, ex.InnerException });
            }
            return base.OnStart();
        }

        public override void Run()
        {
            Trace.TraceInformation("DataReef.SyncEngine.IISHost running");
            base.Run();
        }
    }
}
