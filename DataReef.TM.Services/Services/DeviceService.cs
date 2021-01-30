using System;
using System.Linq;
using DataReef.Core.Infrastructure.Authorization;
using DataReef.Core.Logging;
using System.Data.Entity;
using DataReef.TM.Contracts.Services;
using DataReef.TM.Models;
using DataReef.TM.DataAccess.Database;
using DataReef.Core.Classes;
using System.Collections.Generic;
using DataReef.Integrations.Microsoft;
using System.Threading.Tasks;
using DataReef.Core.Infrastructure.Repository;
using DataReef.Integrations.Microsoft.PowerBI.Models;
using DataReef.Integrations.PushNotification;
using DataReef.TM.Models.PushNotifications;
using DataReef.Core.Enums;
using DataReef.Integrations.PushNotification.DataViews;

namespace DataReef.TM.Services
{
    public class DeviceService : DataService<Device>, IDeviceService
    {
        private Lazy<IPowerBIBridge> _powerBIBridge;
        private Lazy<IAPNPushBridge> _pushBridge;

        public DeviceService(ILogger logger,
            Lazy<IPowerBIBridge> powerBIBridge,
            Func<IUnitOfWork> unitOfWorkFactory,
            Lazy<IAPNPushBridge> pushBridge)
            : base(logger, unitOfWorkFactory)
        {
            _powerBIBridge = powerBIBridge;
            _pushBridge = pushBridge;
        }

        /// <summary>
        /// For a device, the Insert not only inserts a device if it doesnt exists, but it always makes sure there is a UserDevice relationships as well
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public override Device Insert(Device entity)
        {
            Device device;
            using (var context = new DataContext())
            {
                //first see that the device is registered at all
                var d = context.Devices.FirstOrDefault(dd => dd.Uuid == entity.Uuid);

                if (d == null)
                {
                    d = new Device
                    {
                        Uuid = entity.Uuid,
                        Version = entity.Version,
                        TenantID = SmartPrincipal.TenantId,
                        OsVersion = entity.OsVersion,
                        OsName = entity.OsName,
                        Model = entity.Model
                    };
                    Guid g;
                    if (Guid.TryParse(entity.Uuid, out g))
                    {
                        d.Guid = g;
                    }

                    context.Devices.Add(d);
                    device = d;
                }
                else
                {
                    device = d;
                }

                //now check for user device
                var user = context
                                .Users
                                .Include(u => u.UserDevices)
                                .FirstOrDefault(u => u.Guid == SmartPrincipal.UserId);

                var currentDevice = user.UserDevices.FirstOrDefault(dev => dev.DeviceID == d.Guid);

                HandleDevice(context, user.NumberOfDevicesAllowed, user.UserDevices.ToList(), currentDevice, device.Guid, SmartPrincipal.UserId);

                context.SaveChanges();

                //Core requires a saveresult after an insert or any operations
                device.SaveResult = SaveResult.SuccessfulInsert;
            }
            device.UserDevices = null;
            return device;
        }

        public static void HandleDevice(DataContext context, int numberOfDevicesAllowed, List<UserDevice> userDevices, UserDevice currentDevice, Guid deviceId, Guid userId)
        {
            var needToCheckMaxNumber = currentDevice == null || (currentDevice != null && currentDevice.IsDisabled);

            if (needToCheckMaxNumber)
            {
                var numberOfDevicesToDisable = userDevices.Count(d => d.DeviceID != deviceId && !d.IsDisabled) - (numberOfDevicesAllowed - 1);

                if (numberOfDevicesToDisable > 0)
                {
                    var devices = userDevices
                                    .Where(ud => !ud.IsDisabled)
                                    .OrderBy(ud => ud.DateLastModified)
                                    .Take(numberOfDevicesToDisable);

                    foreach (var device in devices)
                    {
                        device.IsDisabled = true;
                        device.DateLastModified = DateTime.UtcNow;
                    }
                }

                // create a new link between user and device
                if (currentDevice == null)
                {
                    var device = context
                                    .Devices
                                    .FirstOrDefault(d => d.Guid == deviceId);

                    if (device == null)
                    {
                        device = new Device
                        {
                            Guid = deviceId,
                            Uuid = deviceId.ToString(),
                            TenantID = SmartPrincipal.TenantId
                        };
                        context.Devices.Add(device);
                    }

                    currentDevice = new UserDevice
                    {
                        UserID = userId,
                        DeviceID = deviceId,
                        IsDisabled = false,
                        DateLastModified = DateTime.UtcNow,
                        LastModifiedBy = userId
                    };
                    context
                        .UserDevices
                        .Add(currentDevice);
                }
                else
                {
                    // reactivate the user device
                    currentDevice.IsDisabled = false;
                    currentDevice.IsDeleted = false;
                    currentDevice.DateLastModified = DateTime.UtcNow;
                }
            }
            else
            {
                currentDevice.Updated(userId);
            }
            context.SaveChanges();
        }

        /// <summary>
        /// Method that verifies that an active UserDevice exists for current UserId and DeviceId
        /// </summary>
        /// <returns></returns>
        public async Task<bool> Validate()
        {
            using (var context = new DataContext())
            {
                var pbi = new PBI_ActiveUser
                {
                    UserID = SmartPrincipal.UserId,
                    DeviceID = SmartPrincipal.DeviceId
                };
               
                await Task.Factory.StartNew(() =>
                {
                    _powerBIBridge.Value.PushData(pbi);
                });

                return context
                        .UserDevices
                        .Any(ud => ud.UserID == SmartPrincipal.UserId
                                && ud.DeviceID == SmartPrincipal.DeviceId
                                && !ud.IsDisabled);
            }
        }

        public void RegisterAPNDeviceToken(string deviceToken)
        {
            using (var uow = UnitOfWorkFactory())
            {
                var devicesSet = uow.Get<Device>();
                var device = devicesSet.FirstOrDefault(d => d.Guid == SmartPrincipal.DeviceId);
                if (device == null)
                {
                    device = new Device
                    {
                        Guid = SmartPrincipal.DeviceId,
                        APNDeviceToken = deviceToken,
                        Uuid = $"{SmartPrincipal.DeviceId}",
                        TenantID = SmartPrincipal.TenantId,
                    };
                    uow.Add(device);

                    var userDevice = new UserDevice
                    {
                        UserID = SmartPrincipal.UserId,
                        DeviceID = device.Guid,
                        IsDisabled = false
                    };
                    uow.Add(userDevice);
                }
                else
                {
                    device.APNDeviceToken = deviceToken;
                }
                uow.SaveChanges(DataSaveOperationContext.Insert);
            }
        }

        public void UnRegisterAPNDeviceToken()
        {
            using (var uow = UnitOfWorkFactory())
            {
                var devicesSet = uow.Get<Device>();
                var device = devicesSet.FirstOrDefault(d => d.Guid == SmartPrincipal.DeviceId);

                if (device != null)
                {
                    device.APNDeviceToken = null;
                }
                uow.SaveChanges(DataSaveOperationContext.Update);
            }
        }

        public List<string> GetAPNDeviceTokens(Guid? userId = null)
        {
            using (var uow = UnitOfWorkFactory())
            {
                return uow
                        .Get<UserDevice>()
                        .Where(ud => ud.UserID == (userId ?? SmartPrincipal.UserId)
                                     && ud.Device.APNDeviceToken != null)
                        .Select(ud => ud.Device.APNDeviceToken)
                        .ToList();
            }
        }

        public List<Tuple<string, NotificationType>> GetPushSubscriptions<T>(string entityId)
        {
            return GetPushSubscriptionsForMultipleIds<T>(new List<string> { entityId });
        }

        public List<Tuple<string, NotificationType>> GetPushSubscriptionsForMultipleIds<T>(List<string> entityIds)
        {
            var entityName = typeof(T).Name;
            using (var uow = UnitOfWorkFactory())
            {
                return uow
                        .Get<PushSubscription>()
                        .Include(ps => ps.Device)
                        .Where(ps => !ps.IsDeleted &&
                                     ps.Name == entityName &&
                                     ps.ExternalID != null &&
                                     entityIds.Contains(ps.ExternalID) &&
                                     ps.DeviceId != SmartPrincipal.DeviceId &&
                                     ps.Device.APNDeviceToken != null)
                        .ToList()
                        .Select(ps => new Tuple<string, NotificationType>(ps.Device.APNDeviceToken, ps.NotificationType))
                        .GroupBy(t => t.Item1)
                        .Select(t => t.FirstOrDefault())
                        .ToList();
            }
        }

        /// <summary>
        /// Send push notifications to subscribers 
        /// </summary>
        /// <typeparam name="S">Subscription type</typeparam>
        /// <typeparam name="P">Payload type</typeparam>
        /// <param name="subscriptionId"></param>
        /// <param name="action"></param>
        /// <param name="parentId"></param>
        /// <param name="alert"></param>
        public void PushToSubscribers<S, P>(string subscriptionId, string payloadId, DataAction action, string parentId = null, string alert = null)
        {
            var deviceTokens = GetPushSubscriptions<S>(subscriptionId);
            if ((deviceTokens?.Count ?? 0) == 0)
            {
                return;
            }

            var pushData = deviceTokens
                            .Select(dt => ApnNotificationDataView.BuildDataView(dt.Item1, ApnPayloadEntityActionDataView.Create<P>(action, payloadId, parentId, dt.Item2 == NotificationType.Silent ? null : alert)))
                            .ToList();

            _pushBridge.Value.PushDataAsync(pushData);
        }
    }

}
