using System;
using System.Diagnostics;
using System.Net;
using System.Threading;
using Microsoft.WindowsAzure.ServiceRuntime;
using DataReef.Core.Infrastructure.Bootstrap;
using DataReef.Queues.QueuesCore;
using DataReef.Queues.Sync;

namespace DataReef.SyncEngine.QueueWorker
{
    public class SyncWorkerRole : RoleEntryPoint
    {
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);
        private QueueMonitor<SyncMessage> syncQueueMonitor;

        public override void Run()
        {
            Trace.TraceInformation("DataReef.SyncEngine.QueueWorker is running");
            try
            {
                var bootstrap = new WorkerRoleBootstrap
                {
                    ProxyNamespaces = new[] { "DataReef.TM.Contracts.Services" }
                };

                bootstrap.Run();

                syncQueueMonitor = new QueueMonitor<SyncMessage>(new SyncQueue());
                syncQueueMonitor.Start();
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

            Trace.TraceInformation("DataReef.SyncEngine.QueueWorker has started");

            return result;
        }

        public override void OnStop()
        {
            Trace.TraceInformation("DataReef.SyncEngine.QueueWorker is stopping");

            syncQueueMonitor.Stop();

            this.cancellationTokenSource.Cancel();
            this.runCompleteEvent.WaitOne();

            base.OnStop();

            Trace.TraceInformation("DataReef.SyncEngine.QueueWorker has stopped");
        }
    }
}
