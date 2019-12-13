using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using Microsoft.WindowsAzure;
using DataReef.Core;
using DataReef.Core.Attributes;
using DataReef.Core.Enums;
using DataReef.TM.Contracts.Services;
using DataReef.Queues.QueuesCore;
using DataReef.Queues.Sync;
using DataReef.Sync.Contracts.Models;
using DataReef.Sync.Services.Database;
using DataReef.Sync.Services.Versioning.Models;
using DataReef.Sync.Services.Versioning.Services;

namespace DataReef.Sync.Services.Queue
{
    public class SyncIdentity : IIdentity
    {
        public string AuthenticationType
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsAuthenticated
        {
            get { return false; }
        }

        public string Name
        {
            get { return "SyncIdentity"; }
        }
    }

    public class SyncPrincipal : IPrincipal
    {
        public IIdentity Identity
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsInRole(string role)
        {
            throw new NotImplementedException();
        }
    }

    [Service(typeof(QueueWorker<SyncMessage>))]
    public class SyncQueueWorker : QueueWorker<SyncMessage>
    {
        private readonly Func<ISyncDataService> _syncDataService;
        private readonly IEntityVersionPersistanceService _versionPersistanceService;

        public SyncQueueWorker(Func<ISyncDataService> syncDataService, IEntityVersionPersistanceService versionPersistanceService)
        {
            _syncDataService = syncDataService;
            _versionPersistanceService = versionPersistanceService;
        }

        public override bool TryProcessMessage(SyncMessage message)
        {
            try
            {
                Trace.TraceInformation("Process sync message: " + message);
                if (!IsValidMessage(message))
                {
                    Trace.TraceWarning("Invalid message", new object[] { message });
                    return false;
                }

                SetThreadIdentity(message.OuGuid, message.TenantId);

                HandlerVersioning(message);
                HandleSync(message);

                return true;
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.Message, ex.StackTrace);
                return false;
            }
        }

        private void HandlerVersioning(SyncMessage message)
        {
            Trace.TraceInformation("Processing versioning " + message);

            try
            {
                var newEntityVersion = new VersioningEntity
                {
                    EntityId = message.EntityGuid,
                    EntityType = message.EntityType,
                    EntityVersion = message.Version,
                    Entity = message.Entity
                };

                _versionPersistanceService.PersistEntityVersion(newEntityVersion);
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.Message + "|" + ex.StackTrace);
            }
        }

        private void HandleSync(SyncMessage message)
        {
            Trace.TraceInformation("Processing sync " + message);

            var proxy = _syncDataService();
            var syncContainers = proxy.GetSyncDevices(message.OuGuid, message.EntityGuid, message.EntityType).ToList();

            if (!syncContainers.Any())
            {
                Trace.TraceWarning("There are no sync containers for " + message);
                return;
            }

            using (var dbConnection = new SqlConnection(ConfigurationManager.ConnectionStrings[CloudConfigurationManager.GetSetting("ConnectionStringName")].ConnectionString))
            {
                if (dbConnection.State != ConnectionState.Open)
                    dbConnection.Open();

                using (var syncDataContext = new SyncDataContext(dbConnection))
                {
                    foreach (var userDevice in syncContainers)
                    {
                        if (userDevice.DeviceID == message.DeviceGuid && userDevice.UserID == message.UserGuid)
                            continue;

                        var existing = syncDataContext.Set<Delta>().FirstOrDefault(s =>
                            s.UserGuid == userDevice.UserID && s.EntityGuid == message.EntityGuid &&
                            s.DeviceGuid == userDevice.DeviceID);

                        if (existing == null)
                        {
                            syncDataContext.Set<Delta>().Add(new Delta
                            {
                                UserGuid = userDevice.UserID,
                                DeviceGuid = userDevice.DeviceID,
                                DataAction = message.DataAction,
                                EntityGuid = message.EntityGuid,
                                EntityType = message.EntityType,
                                Version = message.Version,
                                DateCreated = DateTime.UtcNow,
                                TenantId = message.TenantId,
                                SyncSessionGuid = null,
                            });
                        }
                        else
                        {
                            existing.DataAction = message.DataAction;
                            existing.Version = message.Version;
                        }
                    }

                    syncDataContext.SaveChanges();
                }

                //enqueue a sync notification for this OU
                Core.Notifications.NotificationClient.DefaultInstance.EnqueueSyncNotificationForContainer(message.OuGuid);
            }
            return;
        }

        private static bool IsValidMessage(SyncMessage message)
        {
            if (message == null
             || message.EntityType == null
             || message.DataAction == DataAction.None
             || message.UserGuid == Guid.Empty
             || message.DeviceGuid == Guid.Empty
             || message.OuGuid == Guid.Empty)
                return false;

            return true;
        }

        private static void SetThreadIdentity(Guid ouIdGuid, long tenantId)
        {

            var claimsPrincipal = new ClaimsPrincipal(
                new ClaimsIdentity(
                    new[]
                    {
                        new Claim(SmartClaimTypes.UserId, SystemUsers.SyncService),
                        new Claim(SmartClaimTypes.DeviceId, SystemUsers.SyncService),
                        new Claim(SmartClaimTypes.OuId, ouIdGuid.ToString()),
                        new Claim(SmartClaimTypes.TenantId, tenantId.ToString(CultureInfo.InvariantCulture))
                    })
                );

            Thread.CurrentPrincipal = claimsPrincipal;
        }
    }
}
