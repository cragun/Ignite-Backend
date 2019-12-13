using DataReef.Core.Attributes;
using DataReef.Core.Configuration;
using DataReef.Queues.QueuesCore;

namespace DataReef.Queues.Mail
{
    [Service(typeof(IQueueService<UserCreatedMessage>))]
    public sealed class UserCreatedQueue : QueueService<UserCreatedMessage>
    {
        private const string QueueName = "UserCreatedQueue";

        public UserCreatedQueue() : base(QueueName, CloudConfigurationKeys.StorageAccountConnectionString)
        {
        }
    }
}