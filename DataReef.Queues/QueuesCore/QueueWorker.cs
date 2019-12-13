namespace DataReef.Queues.QueuesCore
{
    public abstract class QueueWorker<T> where T : BaseQueueMessage, new()
    {
        public abstract bool TryProcessMessage(T message);
    }
}
