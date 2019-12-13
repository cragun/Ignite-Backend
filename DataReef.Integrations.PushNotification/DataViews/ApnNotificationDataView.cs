using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.Integrations.PushNotification.DataViews
{
    public class ApnNotificationDataView
    {
        public string DeviceToken { get; set; }
        /// <summary>
        /// The payload serialized as JSON
        /// </summary>
        public string Payload { get; set; }

        private static JsonSerializerSettings SerializationSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore
        };

        public static ApnNotificationDataView BuildDataView(string deviceToken, string payload)
        {
            return new ApnNotificationDataView
            {
                DeviceToken = deviceToken,
                Payload = payload
            };
        }

        public static ApnNotificationDataView BuildDataView(string deviceToken, ApnPayloadEntityActionDataView data)
        {
            return new ApnNotificationDataView
            {
                DeviceToken = deviceToken,
                Payload = JsonConvert.SerializeObject(data, SerializationSettings)
            };
        }

    }
}
