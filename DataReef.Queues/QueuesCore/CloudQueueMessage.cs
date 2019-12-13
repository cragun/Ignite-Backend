using Microsoft.WindowsAzure.Storage.Queue;

namespace DataReef.Queues.QueuesCore
{
    public sealed class QueueMessage<T> where T : BaseQueueMessage, new()
    {
        private readonly T data;
        internal CloudQueueMessage CloudQueueMessage { get; private set; }

        private QueueMessage() { }

        internal QueueMessage(CloudQueueMessage cloudQueueMessage, T data)
        {
            this.data = data;
            this.CloudQueueMessage = cloudQueueMessage;
        }

        public T Data { get { return data; } }
    }
}