using System;
using System.Diagnostics;
using System.Net;
using System.Threading;
using Microsoft.WindowsAzure.ServiceRuntime;
using DataReef.Core.Infrastructure.Bootstrap;
using DataReef.Queues.QueuesCore;
using DataReef.Queues.Mail;

namespace DataReef.Mail
{
    public class WorkerRole : RoleEntryPoint
    {
        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);

        private QueueMonitor<UserCreatedMessage> userCreatedQueueMonitor;


        public override void Run()
        {
            Trace.TraceInformation("DataReef.Mail.Worker is running");

            try
            {
                var bootstrap = new WorkerRoleBootstrap
                {
                    ProxyNamespaces = new[] { "DataReef.TM.Contracts.Services" }
                };

                bootstrap.Run();
                this.userCreatedQueueMonitor = new QueueMonitor<UserCreatedMessage>(new UserCreatedQueue());
                this.userCreatedQueueMonitor.Start();

            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.Message + "|" + ex.StackTrace);
            }
            finally
            {
                this.runCompleteEvent.Set();
            }
        }

        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections
            ServicePointManager.DefaultConnectionLimit = 12;

            // For information on handling configuration changes
            // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.

            bool result = base.OnStart();

            Trace.TraceInformation("DataReef.Mail.Worker has been started");

            return result;
        }

        public override void OnStop()
        {
            Trace.TraceInformation("DataReef.Mail.Worker is stopping");

            this.userCreatedQueueMonitor.Stop();

            Trace.TraceInformation("DataReef.Mail.Worker has stopped");
        }
    }


}
