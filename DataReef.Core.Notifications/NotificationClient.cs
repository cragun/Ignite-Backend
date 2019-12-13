using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Newtonsoft.Json;
using Microsoft.ServiceBus.Notifications;



namespace DataReef.Core.Notifications
{
    public class NotificationClient
    {
        /// <summary>
        /// private singleton instance
        /// </summary>
        private static NotificationClient instance;

        /// <summary>
        /// how long will the Sync Message wait for inactivity before processing
        /// </summary>
        private const double SyncDelay = 1000 * 15;

        /// <summary>
        /// Time that fires every second processing sync messages
        /// </summary>
        private readonly System.Timers.Timer _timer;

        /// <summary>
        /// used to batch up sync requests... after the DateTime they can be processed by the Timer_Elapsed
        /// </summary>
        private readonly Dictionary<Guid, DateTime> _syncQueue = new Dictionary<Guid, DateTime>();

        /// <summary>
        /// Private Constructor, must use Singleton pattern due to the Timer and needing to make sure there is only ONE notification processor
        /// </summary>
        private NotificationClient()
        {
            _timer = new System.Timers.Timer(1000)
            {
                AutoReset = true
            };
            _timer.Elapsed += timer_Elapsed;
            _timer.Start();
        }

        /// <summary>
        /// loops through the queue to see if there are any Organization Units (Guids - Keys) that can be pulled out and processed if the Value (Sex) is less than System.DateTime.UTCNow
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            _timer.Stop();

            lock (this)
            {
                foreach (var kv in _syncQueue.ToList())
                {
                    if (kv.Value <= DateTime.UtcNow) continue;
                    _syncQueue.Remove(kv.Key);
                    SendSyncNotificationsForContainer(kv.Key);
                }
            }

            _timer.Start();
        }

        /// <summary>
        /// Singleton
        /// </summary>
        public static NotificationClient DefaultInstance
        {
            get { return instance ?? (instance = new NotificationClient()); }
        }


        public async void SendTemplateNotification(string message, string hubName, string tagName)
        {

            var fullToken = System.Configuration.ConfigurationManager.AppSettings["Microsoft.ServiceBus.ConnectionString"];
            var hub = NotificationHubClient.CreateClientFromConnectionString(fullToken, hubName);

            var dic = new Dictionary<string, string>
            {
                {"message", message}
            };

            if (string.IsNullOrWhiteSpace(tagName) == false)
            {
                await hub.SendTemplateNotificationAsync(dic, tagName);
            }
            else
            {
                await hub.SendTemplateNotificationAsync(dic);
            }
        }

        /// <summary>
        /// Creates a notification that the Apple IOS can process
        /// </summary>
        /// <param name="notification"></param>
        public async void SendApplicationNotification(AppleNotification notification)
        {
            var dic = new Dictionary<string, object>
            {
                {"aps", notification.Payload}
            };
            foreach (var kv in notification.Data)
            {
                dic.Add(kv.Key, kv.Value);
            }

            var payload = JsonConvert.SerializeObject(dic, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
            var fullToken = System.Configuration.ConfigurationManager.AppSettings["Microsoft.ServiceBus.ConnectionString"];
            notification.Hub = string.IsNullOrWhiteSpace(notification.Hub) ? "tmnotifications" : notification.Hub;
            var hub = NotificationHubClient.CreateClientFromConnectionString(fullToken, notification.Hub);
            try
            {
                if (notification.Tags.Any())
                {
                    foreach (var tag in notification.Tags)
                    {
                        await hub.SendAppleNativeNotificationAsync(payload, tag);
                    }
                }
                else
                {
                    await hub.SendAppleNativeNotificationAsync(payload);
                }
            }
            catch (Exception exception)
            {

                Trace.TraceError("An error has occurred while sending an Apple notification. Stack Message : {0 } \n Stack Trace : {1}", exception.Message, exception.StackTrace); ;
            }



        }

        /// <summary>
        /// This is THE method that sync server needs to call to process a sync message for a container (OU).  adds or updates the Container (Guid) to be processed for a sync notification in 15 seconds
        /// </summary>
        /// <param name="containerGuid"></param>
        public void EnqueueSyncNotificationForContainer(Guid containerGuid)
        {
            lock (this)
            {
                _syncQueue[containerGuid] = DateTime.UtcNow.AddSeconds(SyncDelay);
            }

        }


        private void SendSyncNotificationsForContainer(Guid containerGuid)
        {
            var an = new AppleNotification();
            an.Data.Add("type", "sync");
            an.Data.Add("guid", containerGuid.ToString());
            an.Tags.Add("sync-" + containerGuid);
            SendApplicationNotification(an);

            //todo create and send android
            var androidMessage = new AndroidNotification();

            androidMessage.Data.Add("type", "sync");
            androidMessage.Data.Add("guid", containerGuid.ToString());
            androidMessage.Tags.Add("sync-" + containerGuid);


            SendAndroidNotification(androidMessage);
        }

        private async void SendAndroidNotification(AndroidNotification notification)
        {
            var properties = new Dictionary<string, object>();
            foreach (var kv in notification.Data)
            {
                properties.Add(kv.Key, kv.Value);
            }
            var message = JsonConvert.SerializeObject(properties, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });


            var payload =
                "{\"data\" : " +
                "   {" +
                "   \"message\": \"" + message + "\"," +
                "   }" +
                "}";

            const string fullToken = "Endpoint=sb://smartcare.servicebus.windows.net/;SharedAccessKeyName=DefaultFullSharedAccessSignature;SharedAccessKey=r6JLm/p5rhGDzWGuihXmIedvTf02hq6TdNo57VK8XkE=";
            notification.Hub = string.IsNullOrWhiteSpace(notification.Hub) ? "smartcarenotifications" : notification.Hub;
            var hub = NotificationHubClient.CreateClientFromConnectionString(fullToken, notification.Hub);
            try
            {
                if (notification.Tags.Any())
                {
                    await hub.SendGcmNativeNotificationAsync(payload, notification.Tags);
                }
                else
                {
                    await hub.SendGcmNativeNotificationAsync(payload);
                }
            }

            catch (Exception exception)
            {
                Trace.TraceError("An error has occurred while sending an Android notification. Stack Message : {0 } \n Stack Trace : {1}", exception.Message, exception.StackTrace);
            }

        }


    }
}
