using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using DataReef.Core.Infrastructure.Authorization;
using DataReef.TM.Contracts.Services;
using DataReef.TM.DataAccess.Database;
using DataReef.TM.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace DataReef.TM.Services.Services
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(AddressFilterMode = AddressFilterMode.Any)]
    public class SmsService : ISmsService
    {
        private static readonly string _s3AccessKeyId = ConfigurationManager.AppSettings["AWS_S3_AccessKeyID"];
        private static readonly string _s3SecretAccessKey = ConfigurationManager.AppSettings["AWS_S3_SecretAccessKey"];

        public void SendSms(string message, string mobileNumber)
        {
            try
            {
                AmazonSimpleNotificationServiceClient snsClient = new AmazonSimpleNotificationServiceClient(_s3AccessKeyId, _s3SecretAccessKey, Amazon.RegionEndpoint.USWest2);
                PublishRequest pubRequest = new PublishRequest();
                pubRequest.Message = message;

                mobileNumber = mobileNumber.Replace("()", "").Trim();
                if (!mobileNumber.Contains("+1"))
                {
                    mobileNumber = $"+1 {mobileNumber}";
                }

                pubRequest.PhoneNumber = mobileNumber;
                PublishResponse pubResponse = snsClient.Publish(pubRequest);
            }
            catch (Exception ex)
            {
                throw new Exception($"{ex.Message} {ex.StackTrace}");
            }

        }
    }
}
