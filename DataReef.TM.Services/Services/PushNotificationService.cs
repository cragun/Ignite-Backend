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
using System.Threading.Tasks;

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
                httpWebRequest.Headers.Add("Authorization", "key=" + serverKey);
                httpWebRequest.Method = "POST";

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    string json = "{\"to\": \"" + token + "\",\"notification\": {\"title\": \""+ title +"\",\"body\": \""+ message+ "\"},\"priority\":10}"; 
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
