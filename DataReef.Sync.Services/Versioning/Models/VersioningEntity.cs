using System;

namespace DataReef.Sync.Services.Versioning.Models
{
    public class VersioningEntity
    {
        public Guid EntityId { get; set; }

        public int EntityVersion { get; set; }

        public string EntityType { get; set; }

        public object Entity { get; set; }
    }
}