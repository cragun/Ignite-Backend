using System;
using System.Collections.Generic;

namespace DataReef.Queues.QueuesCore
{
    public interface IQueueService<T> where T : BaseQueueMessage, new()
    {
        void Enqueue(T message);

        QueueMessage<T> Dequeue(int invisibilityTimeoutSeconds = 30);
        IEnumerable<QueueMessage<T>> DequeueMany(int invisibilityTimeoutSeconds = 30, int numberOfMessages = 20);

        void Delete(params QueueMessage<T>[] msg);

        int DequeueAndProcess(Func<QueueMessage<T>, bool> processMessage, int invisibilityTimeoutSeconds = 30, int numberOfMessages = 20);

        bool HasMessages();
    }
}