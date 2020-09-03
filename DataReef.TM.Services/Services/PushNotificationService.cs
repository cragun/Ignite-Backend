using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using DataReef.TM.Contracts.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Text;

namespace DataReef.TM.Services.Services
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(AddressFilterMode = AddressFilterMode.Any)]
    public class PushNotificationService : IPushNotificationService
    {
        private static readonly string serverKey = ConfigurationManager.AppSettings["Firebase.ServerKey"]; 
        private static readonly string url = "https://fcm.googleapis.com/fcm/send"; 

        public string PushNotification(string token, string message, string title)
        { 
            try
            {
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Headers.Add("Authorization", "key=AAAAjcK0I_g:APA91bE9yx0Ximczoh423GN5fUhOSG5XOYnLxHDJtciBdGcapueC9LhCe0xyMMJwnfY79UrZ83rPXhbQLvW_JOcbbT6xNy_P7U96YKfQXB_U2Zr5Um58Dk0TglI_pvRghEoll5AqfN94");
                httpWebRequest.Method = "POST";

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    string json = "{\"to\": \"" + "cCgFluq1nE9QmEhmwJcPQI:APA91bEiWuHQbmgATcf2KBfCM37O3OeLPZyV9WOZrDT8i0w7f0KG4f_funNkTH-jCyFnBUW3N5eseDw5grZwfBEi4LoNBNwlmRf_w40z_qAqIYLlXqQLYhEdODm9M7X93oMchCKIIt9C" + "\",\"notification\": {\"title\": \"" + title + "\",\"body\": \"" + message + "\"},\"priority\":10}";
                    streamWriter.Write(json);
                    streamWriter.Flush();
                }

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                var result = "-1";
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    result = streamReader.ReadToEnd();
                }

                return result;

            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
        }
    }
}
