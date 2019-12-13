using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Practices.ServiceLocation;

namespace DataReef.Queues.QueuesCore
{
    public sealed class QueueMonitor<T> where T : BaseQueueMessage, new()
    {
        private readonly int _queuePeakIntervalSeconds;
        private readonly int _meessagesNumberToDequeue;
        private readonly short _maxThrottleMultiplier;

        private readonly IQueueService<T> _queueService;

        private CancellationTokenSource _cancellationToken;

        public QueueMonitor(IQueueService<T> queueService, int queuePeakIntervalSeconds = 30, int meessagesNumberToDequeue = 20, short maxThrottleMultiplier = 10)
        {
            _queueService = queueService;
            _meessagesNumberToDequeue = meessagesNumberToDequeue;
            _queuePeakIntervalSeconds = queuePeakIntervalSeconds;
            _maxThrottleMultiplier = maxThrottleMultiplier;
        }

        public void Start()
        {
            _cancellationToken = new CancellationTokenSource();
            var throttleMultiplier = 1;
            while (!_cancellationToken.IsCancellationRequested)
            {
                Thread.Sleep(_queuePeakIntervalSeconds * 1000 * throttleMultiplier);

                var sucessContinuation = Task<bool>.Factory
                    .StartNew(DoWork, _cancellationToken.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default)
                    .ContinueWith(task =>
                    {
                        // if had no work and the throttle multiplier is smaller the target multiplier throttle the work thread
                        if (!task.Result && throttleMultiplier < _maxThrottleMultiplier)
                            throttleMultiplier++;

                        // if had work and the throttle multiplier is not 1 reset the multiplier
                        if (task.Result && throttleMultiplier != 1)
                            throttleMultiplier = 1;
                    }, _cancellationToken.Token, TaskContinuationOptions.OnlyOnRanToCompletion, TaskScheduler.Current);

                var failiureContinuation = sucessContinuation.ContinueWith(task =>
                {
                    if (task == null || task.Exception == null)
                        return;

                    var exceptions = task.Exception.Flatten().InnerExceptions;
                    if (exceptions == null)
                        return;

                    foreach (var exception in exceptions)
                        Trace.TraceError(exception.Message + "|" + exception.StackTrace);
                }, _cancellationToken.Token, TaskContinuationOptions.OnlyOnFaulted, TaskScheduler.Current);

                Task.WaitAny(sucessContinuation, failiureContinuation);
            }
        }

        public void Stop()
        {
            if (_cancellationToken.IsCancellationRequested)
                return;

            _cancellationToken.Cancel();
        }

        private bool DoWork()
        {
            var hadWork = false;

            while (true)
            {
                var messageCount = _queueService.DequeueAndProcess(queueMessage =>
                {
                    var queueWorker = ServiceLocator.Current.GetInstance<QueueWorker<T>>();
                    if (queueWorker == null)
                        throw new Exception("ServiceLocator.Current could not construct QueueWorker instance");

                    return queueWorker.TryProcessMessage(queueMessage.Data);
                }, numberOfMessages: _meessagesNumberToDequeue);

                if (messageCount != 0 && hadWork == false)
                    hadWork = true;

                if (messageCount < _meessagesNumberToDequeue || _cancellationToken.IsCancellationRequested || !_queueService.HasMessages())
                    break;
            }

            return hadWork;
        }
    }
}