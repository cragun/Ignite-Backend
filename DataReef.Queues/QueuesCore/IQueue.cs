using System;
using System.Collections.Generic;
using Microsoft.WindowsAzure.Storage.Queue;

namespace DataReef.Queues.QueuesCore
{
    /// <summary>
    /// This interfaces needs some work. The CloudQueueMessage needs to be abstracted
    /// The main reason for this interface is to be able to initialize a fake IQueue that dose not require Azure to work. 
    /// So the application can work in the dev environment where the local azure emulator might not be running.
    /// </summary>
    internal interface IQueue
    {
        CloudQueueMessage GetMessage(TimeSpan? visibilityTimeout);
        IEnumerable<CloudQueueMessage> GetMessages(int numberOfMessages, TimeSpan? timeSpan);
        void AddMessage(CloudQueueMessage message);
        void DeleteMessage(CloudQueueMessage message);
        CloudQueueMessage PeekMessage();
    }
}