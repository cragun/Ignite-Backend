using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.Integrations.PushNotification.DataViews
{
    public class ApnPayloadBaseDataView
    {
        [JsonProperty("aps")]
        public ApnAPS APS { get; set; }

    }

    public class ApnAPS
    {
        [JsonProperty("content-available")]
        public int? ContentAvailable { get; set; }

        [JsonProperty("alert")]
        public string Alert { get; set; }

        public ApnAPS()
        { }

        public ApnAPS(string alert)
        {
            // silent notification
            if (alert == null)
            {
                ContentAvailable = 1;
            }
            else
            {
                Alert = alert;
            }
        }
    }
}
