using System;

namespace DataReef.Queues.QueuesCore
{
    [Serializable]
    public abstract class BaseQueueMessage
    {
        public DateTime DateCreated { get; set; }
    }
}