using System.Collections.Generic;
using Newtonsoft.Json;

namespace DataReef.Core.Notifications
{
    public class Alert
    {
        public Alert()
        {
            this.LocArgs = new List<string>();
        }

        [JsonProperty("body")]
        public string Body { get; set; }

        [JsonProperty("launch-image")]
        public string LaunchImage { get; set; }

        [JsonProperty("action-loc-key")]
        public string ActionLocKey { get; set; }

        [JsonProperty("loc-key")]
        public string LocKey { get; set; }


        [JsonProperty("loc-args")]
        public List<string> LocArgs { get; set; }
      
    }
    public class AlertPayload
    {
        public AlertPayload()
        {
            this.Alert = new Alert();
        }
        
        [JsonProperty("alert")]
        public Alert Alert { get; set; }

        
        [JsonProperty("badge")]
        public int? Badge { get; set; }

        [JsonProperty("sound")]
        public string Sound { get; set; }

        [JsonProperty("content-available")]
        public bool? ContentAvailable { get; set; }
    }


    public class AppleNotification
    {
        public AppleNotification()
        {
            this.Payload = new AlertPayload();
            this.Tags = new List<string>();
            this.Data = new Dictionary<string, string>();
        }

        public AppleNotification(string message,string hub,string tag)
        {
            this.Payload = new AlertPayload();
            this.Tags = new List<string>() { tag };
            this.Data = new Dictionary<string, string>();

            this.Payload.Alert.Body = message;
            this.Hub = hub;

        }

        public AlertPayload Payload { get; set; }
        public string Hub { get; set; }
        public List<string> Tags { get; set; }
        public Dictionary<string, string> Data;

    }
}
