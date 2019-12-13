using System;
using System.Collections.Generic;
using Microsoft.WindowsAzure.Storage.Queue;

namespace DataReef.Queues.QueuesCore
{
    internal class AzureQueue : IQueue
    {
        private readonly CloudQueue cloudQueue;

        public AzureQueue(CloudQueue cloudQueue)
        {
            this.cloudQueue = cloudQueue;
        }


        public CloudQueueMessage GetMessage(TimeSpan? visibilityTimeout)
        {
            return this.cloudQueue.GetMessage(visibilityTimeout);
        }

        public IEnumerable<CloudQueueMessage> GetMessages(int numberOfMessages, TimeSpan? timeSpan)
        {
            return this.cloudQueue.GetMessages(numberOfMessages, timeSpan);
        }

        public void AddMessage(CloudQueueMessage message)
        {
            this.cloudQueue.AddMessage(message);
        }

        public void DeleteMessage(CloudQueueMessage message)
        {
            this.cloudQueue.DeleteMessage(message);
        }

        public CloudQueueMessage PeekMessage()
        {
            return this.cloudQueue.PeekMessage();
        }
    }
}