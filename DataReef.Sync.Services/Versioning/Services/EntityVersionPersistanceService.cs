using System;
using System.Diagnostics;
using System.Globalization;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using DataReef.Core;
using DataReef.Core.Attributes;
using DataReef.Core.Configuration;
using DataReef.Sync.Services.Versioning.Exceptions;
using DataReef.Sync.Services.Versioning.Models;

namespace DataReef.Sync.Services.Versioning.Services
{
    [Service(typeof(IEntityVersionPersistanceService))]
    internal class EntityVersionPersistanceService : IEntityVersionPersistanceService
    {
        const string TableName = "EntityVersioning";

        public void PersistEntityVersion(VersioningEntity versioningEntity)
        {
            try
            {
                var table = GetVersioningTable();
                table.CreateIfNotExists();

                // Lookup in azure table happens using partition key then the RowKey in that descending order.
                // Smaller values are placed first thus we store the time till the end of DateTime and so the most recent entry will be the first entry.
                // Would be nice to use this trick but we have decided to use the versioningEntity version as the RowKey. 
                // If more performance is needed from the versioning lookup logic find a composite key with this timespan in it.
                //string rowKey = string.Format(CultureInfo.InvariantCulture, "{0:d19}", DateTime.MaxValue.Ticks - DateTime.UtcNow.Ticks);

                var partitionKey = versioningEntity.EntityId.ToString();
                var rowKey = versioningEntity.EntityVersion.ToString(CultureInfo.InvariantCulture);

                var newVersionedEntity = new VersioningTableEntity(partitionKey, rowKey)
                {
                    EntityId = versioningEntity.EntityId,
                    EntityVersion = versioningEntity.EntityVersion,
                    EntityFullName = versioningEntity.EntityType,
                    EntityJson = JsonConvert.SerializeObject(versioningEntity.Entity, new JsonSerializerSettings
                    {
                        Converters = new JsonConverter[] {new GuidRefJsonConverter(true)},
                        TypeNameHandling = TypeNameHandling.All
                    })
                };

                TableOperation insertOrReplaceOperation = TableOperation.InsertOrReplace(newVersionedEntity);

                table.Execute(insertOrReplaceOperation);
            }
            catch (Exception ex)
            {
                Trace.TraceError("Could not persist in versioning table " + ex.Message +"| " + ex.StackTrace);
            }
        }

        public VersioningEntity RetreaveEntityVersion(Guid entityId, int version)
        {
            var table = GetVersioningTable();
            TableOperation retrieveOperation = TableOperation.Retrieve<VersioningTableEntity>(entityId.ToString(), version.ToString(CultureInfo.InvariantCulture));

            TableResult retrievedResult = table.Execute(retrieveOperation);

            if (retrievedResult.Result != null)
            {
                var tableEntity = (VersioningTableEntity)retrievedResult.Result;
                return new VersioningEntity
                {
                    Entity = JsonConvert.DeserializeObject(tableEntity.EntityJson, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All }),
                    EntityType = tableEntity.EntityFullName,
                    EntityVersion = tableEntity.EntityVersion,
                    EntityId = tableEntity.EntityId
                };
            }

            throw new RetrieveVersionedEntityException("Failed to find version: " + version + " for versioningEntity: " + entityId);
        }

        private static CloudTable GetVersioningTable()
        {

            CloudStorageAccount storageAccount =
                CloudStorageAccount.Parse(
                    CloudConfigurationManager.GetSetting(CloudConfigurationKeys.StorageAccountConnectionString));

            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference(TableName);
            return table;
        }
    }
}