using System;
using DataReef.Queues.QueuesCore;

namespace DataReef.Queues.Mail
{
    [Serializable]
    public class UserCreatedMessage : BaseQueueMessage
    {
        public Guid OuID { get; set; }

        public string OuName { get; set; }

        public string RoleName { get; set; }

        public Guid UserID { get; set; }

        public Guid? DeviceID { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public long TenantID { get; set; }
    }
}