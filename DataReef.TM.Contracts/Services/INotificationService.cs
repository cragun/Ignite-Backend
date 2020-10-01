using System.ServiceModel;
using DataReef.TM.Models;
using System.Collections.Generic;
using System;

namespace DataReef.TM.Contracts.Services
{
    [ServiceContract]
    [ServiceKnownType("GetKnownTypes", typeof(KnownTypesProvider))]
    public interface INotificationService : IDataService<Notification>
    {
        [OperationContract]
        IEnumerable<Notification> GetNotificationsForPerson(Guid PersonID, IgniteNotificationSeenStatus? status = null, int pageNumber = 1, int itemsPerPage = 10);

        [OperationContract]
        Notification MarkAsRead(Guid notificationID);

        [OperationContract]
        IEnumerable<Notification> MarkAllNotificationsAsRead(Guid personID, int pageNumber = 0, int itemsPerPage = 10);

        [OperationContract]
        int CountUnreadNotifications(Guid personID);

        [OperationContract]
        bool MarkAsReadFromSmartboard(string notificationSmartboardID, string apiKey);
    }
}