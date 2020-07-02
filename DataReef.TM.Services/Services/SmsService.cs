using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using DataReef.TM.Contracts.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Services.Services
{
    public class SmsService : ISmsService
    {
        public void SendSms()
        {
            AmazonSimpleNotificationServiceClient snsClient = new AmazonSimpleNotificationServiceClient(Amazon.RegionEndpoint.USWest2);
            PublishRequest pubRequest = new PublishRequest();
            pubRequest.Message = "My SMS message";
            pubRequest.PhoneNumber = "+919314126197";
            // add optional MessageAttributes, for example:
            //   pubRequest.MessageAttributes.Add("AWS.SNS.SMS.SenderID", new MessageAttributeValue
            //      { StringValue = "SenderId", DataType = "String" });
            PublishResponse pubResponse = snsClient.Publish(pubRequest);
            Console.WriteLine(pubResponse.MessageId);
        }
    }
}
