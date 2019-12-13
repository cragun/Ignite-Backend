using DataReef.Core.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Text;
using System.Threading.Tasks;
using DataReef.Integrations.PushNotification.DataViews;
using PushSharp.Apple;
using System.Configuration;
using static PushSharp.Apple.ApnsConfiguration;
using System.IO;
using System.Reflection;
using Newtonsoft.Json.Linq;

namespace DataReef.Integrations.PushNotification
{

    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(AddressFilterMode = AddressFilterMode.Any)]
    [Service(typeof(IAPNPushBridge))]
    public class APNPushBridge : IAPNPushBridge
    {
        private static readonly ApnsServerEnvironment APNEnvironment = (ApnsServerEnvironment)Enum.Parse(typeof(ApnsServerEnvironment), ConfigurationManager.AppSettings["Apple.PushNotification.Environment"] ?? "Sandbox");

        private static readonly string PushCertPWD = ConfigurationManager.AppSettings[$"Apple.PushNotification.Certificate.{APNEnvironment}.Password"];

        private static byte[] _pushCert;
        private static byte[] PushCert
        {
            get
            {
                if (_pushCert == null)
                {
                    var path = ConfigurationManager.AppSettings["Apple.PushNotification.Certificate.Path"];
                    if (path.StartsWith("~"))
                    {
                        path = path.Replace("~", "..\\..");
                        path = new Uri(Path.Combine(Assembly.GetExecutingAssembly().CodeBase, path)).LocalPath;
                    }
                    if (!File.Exists(path))
                    {
                        throw new ApplicationException("Couldn't find APN Push Certificate!");
                    }
                    _pushCert = File.ReadAllBytes(path);
                }
                return _pushCert;
            }
        }

        private static ApnsConfiguration _apnConfig;
        private static ApnsConfiguration APNconfig
        {
            get
            {
                if (_apnConfig == null)
                {
                    try
                    {
                        _apnConfig = new ApnsConfiguration(APNEnvironment, PushCert, PushCertPWD);
                    }
                    catch
                    {
                        throw new ApplicationException("Cannot create APN Broker!");
                    }
                }
                return _apnConfig;
            }
        }

        private ApnsServiceBroker NewBroker()
        {
            return new ApnsServiceBroker(APNconfig);
        }

        public void PushData(string deviceToken, string payload)
        {
            PushDataAsync(deviceToken, payload).Wait();
        }

        public void PushData(List<ApnNotificationDataView> data)
        {
            PushDataAsync(data).Wait();
        }

        public async Task PushDataAsync(string deviceToken, string payload)
        {
            if (string.IsNullOrEmpty(deviceToken) || string.IsNullOrEmpty(payload))
            {
                return;
            }
            await PushDataAsync(new List<ApnNotificationDataView> { ApnNotificationDataView.BuildDataView(deviceToken, payload) });
        }

        public async Task PushDataAsync(List<ApnNotificationDataView> data)
        {
            if (data == null)
                return;

            var apnBroker = NewBroker();

            apnBroker.OnNotificationFailed += (notification, aggregateEx) =>
            {
                aggregateEx.Handle(ex =>
                {
                    // See what kind of exception it was to further diagnose
                    if (ex is ApnsNotificationException)
                    {
                        var notificationException = (ApnsNotificationException)ex;

                        // Deal with the failed notification
                        var apnsNotification = notificationException.Notification;
                        var statusCode = notificationException.ErrorStatusCode;

                        Console.WriteLine($"Apple Notification Failed: ID={apnsNotification.Identifier}, Code={statusCode}. IsTokenValid={notification.IsDeviceRegistrationIdValid()}");
                    }
                    else
                    {
                        // Inner exception might hold more useful information like an ApnsConnectionException			
                        Console.WriteLine($"Apple Notification Failed for some unknown reason : {ex.InnerException}");
                    }

                    // Mark it as handled
                    return true;
                });
            };

            apnBroker.Start();
            foreach (var notification in data)
            {
                // Queue a notification to send
                apnBroker.QueueNotification(new ApnsNotification
                {
                    DeviceToken = notification.DeviceToken,
                    Payload = JObject.Parse(notification.Payload)
                });
            }

            // Stop the broker, wait for it to finish   
            // This isn't done after every message, but after you're
            // done with the broker
            apnBroker.Stop();
            await Task.FromResult(true);
        }

        public void Test(string token, string payload)
        {
            PushData(token, payload);
        }
    }
}
