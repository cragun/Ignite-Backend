using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace DataReef.Sync.Services.Versioning.Models
{
    internal class VersioningTableEntity : TableEntity
    {
        public VersioningTableEntity()
        {
        }

        public VersioningTableEntity(string partitionKey, string rowKey)
            : base(partitionKey, rowKey)
        {
        }

        public Guid EntityId { get; set; }
        public int EntityVersion { get; set; }
        public string EntityFullName { get; set; }
        public string EntityJson { get; set; }
    }
}