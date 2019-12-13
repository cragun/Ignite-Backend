using DataReef.Integrations.PushNotification.DataViews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Web.ApplicationServices;

namespace DataReef.Integrations.PushNotification
{
    [ServiceContract]
    [ServiceKnownType("GetKnownTypes", typeof(KnownTypesProvider))]
    public interface IAPNPushBridge
    {
        void PushData(string deviceToken, string payload);
        Task PushDataAsync(string deviceToken, string payload);

        void PushData(List<ApnNotificationDataView> data);
        Task PushDataAsync(List<ApnNotificationDataView> data);

        void Test(string token, string payload);
    }
}
