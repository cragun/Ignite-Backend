using DataReef.Core.Attributes;
using DataReef.Core.Configuration;
using DataReef.Queues.QueuesCore;

namespace DataReef.Queues.Sync
{
    [Service(typeof(IQueueService<SyncMessage>))]
    public sealed class SyncQueue : QueueService<SyncMessage>
    {
        private const string QueueName = "SyncQueue";

        public SyncQueue()
            : base(QueueName, CloudConfigurationKeys.StorageAccountConnectionString)
        {
        }
    }
}