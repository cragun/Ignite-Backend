using System;
using DataReef.Core.Classes;
using DataReef.Core.Enums;
using DataReef.Queues.QueuesCore;

namespace DataReef.Queues.Sync
{
    [Serializable]
    public class SyncMessage : BaseQueueMessage
    {
        public Guid OuGuid { get; set; }

        public Guid UserGuid { get; set; }

        public Guid DeviceGuid { get; set; }

        public string EntityType { get; set; }

        public Guid EntityGuid { get; set; }

        public DataAction DataAction { get; set; }

        public int Version { get; set; }

        public long TenantId { get; set; }

        public DbEntity Entity { get; set; }

        public override string ToString()
        {
            return string.Format(@"EntityType {0} | UserGuid {1} | DeviceGuid {2} | OuGuid {3} | EntityGuid {4} | DataAction {5} | DateCreated {6} | Version {7}"
                                , EntityType, UserGuid, DeviceGuid, OuGuid, EntityGuid, DataAction, DateCreated, Version);
        }
    }
}