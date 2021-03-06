using DataReef.Core.Infrastructure.Repository;
using DataReef.Core.Logging;
using DataReef.TM.Contracts.Services;
using DataReef.TM.DataAccess.Database;
using DataReef.TM.Models;
using DataReef.TM.Models.DTOs;
using DataReef.TM.Models.DTOs.Integrations;
using DataReef.TM.Services.Services.FinanceAdapters.SolarSalesTracker;
using System;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using DataReef.Core.Infrastructure.Authorization;
using DataReef.Core.Classes;
using System.Net.Http;
using System.Configuration;

using RestSharp;
using System.Net;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.IO;

namespace DataReef.TM.Services.Services
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(AddressFilterMode = AddressFilterMode.Any)]
    public class NotificationService : DataService<Notification>, INotificationService
    {

        private readonly Lazy<ISolarSalesTrackerAdapter> _sbAdapter;
        private readonly Lazy<IOUSettingService> _ouSettingService;

        public NotificationService(
            ILogger logger,
            Func<IUnitOfWork> unitOfWorkFactory,
            Lazy<ISolarSalesTrackerAdapter> sbAdapter,
            Lazy<IOUSettingService> ouSettingService) : base(logger, unitOfWorkFactory)
        {
            _sbAdapter = sbAdapter;
            _ouSettingService = ouSettingService;
        }


        public IEnumerable<Notification> MarkAllNotificationsAsRead(Guid personID, int pageNumber = 0, int itemsPerPage = 10)
        {
            using (var dc = new DataContext())
            {
                var unseenNotifications = dc.Notifications.Where(x => !x.IsDeleted && x.PersonID == personID && x.Status == IgniteNotificationSeenStatus.NotSeen).ToList();

                unseenNotifications.ForEach(x =>
                {
                    x.Status = IgniteNotificationSeenStatus.Seen;
                    x.SeenAt = DateTime.UtcNow;
                });

                dc.SaveChanges();

                return dc.Notifications.Where(x => !x.IsDeleted && x.PersonID == personID).OrderByDescending(x => x.DateCreated).Skip(pageNumber * itemsPerPage).Take(itemsPerPage).ToList();
            }
        }

        public IEnumerable<Notification> GetNotificationsForPerson(Guid personID, IgniteNotificationSeenStatus? status = null, int pageNumber = 0, int itemsPerPage = 10)
        {
            using (var dc = new DataContext())
            {
                //var query = dc.Notifications.Include(a => a.Property).Where(x => !x.IsDeleted && x.PersonID == personID).AsNoTracking().ToList();
                var query = dc.Notifications.Where(x => !x.IsDeleted && x.PersonID == personID).AsNoTracking().ToList();

                query.ForEach(a => a.Property = dc.Properties.FirstOrDefault(x => !x.IsDeleted && x.Guid == a.PropertyID));

                if (status.HasValue)
                {
                    query = query.Where(x => x.Status == status.Value).ToList();
                } 

                return query
                    .GroupBy(x => x.Value)
                    .Select(g => g.OrderByDescending(x => x.DateCreated).FirstOrDefault())
                    .OrderByDescending(x => x.DateCreated)
                    .Skip(pageNumber * itemsPerPage)
                    .Take(itemsPerPage);
            }
        }

        public async Task<int> CountUnreadNotifications(Guid personID)
        {
            using (var dc = new DataContext())
            {
                var person = await dc.People.Include(x => x.PersonSettings).AsNoTracking().FirstOrDefaultAsync(x => x.Guid == personID);
                if (person == null)
                {
                    return 0;
                }

                var query = dc
                    .Notifications
                    .Where(x => x.PersonID == personID && !x.IsDeleted && x.Status == IgniteNotificationSeenStatus.NotSeen).AsNoTracking()
                    .GroupBy(x => x.Value)
                    .Select(g => g.OrderByDescending(x => x.DateCreated).FirstOrDefault());

                var lastReadDate = person.PersonSettings.FirstOrDefault(x => x.Name == "Ignite.Notifications.LastChecked");

                if (!string.IsNullOrEmpty(lastReadDate?.Value))
                {
                    if (DateTime.TryParse(lastReadDate?.Value, out var dateLastChecked))
                    {
                        query = query.Where(x => x.DateCreated >= dateLastChecked).AsNoTracking();
                    }

                }

                return query.Count();
            }
        }

        public Notification MarkAsRead(Guid notificationID)
        {
            using (var dc = new DataContext())
            {
                var notification = dc.Notifications.FirstOrDefault(x => !x.IsDeleted && x.Guid == notificationID);
                if (notification == null)
                {
                    return null;
                }

                notification.Status = IgniteNotificationSeenStatus.Seen;
                notification.SeenAt = DateTime.UtcNow;

                //check if the notification is a propertyNote
                if (notification.Value.HasValue)
                {
                    var propertyNote = dc.PropertyNotes.Include(x => x.Property).Include(x => x.Property.Territory).FirstOrDefault(x => x.Guid == notification.Value);
                    if (propertyNote != null && propertyNote?.Property?.Territory?.OUID != null)
                    {
                        //call smartboard to mark their notification as dismissed as well
                        _sbAdapter.Value.DismissNotification(propertyNote.Property.Territory.OUID, notification.SmartBoardID);
                    }

                }


                dc.SaveChanges();

                return notification;
            }
        }

        public bool MarkAsReadFromSmartboard(string notificationSmartboardID, string apiKey)
        {
            using (var dc = new DataContext())
            {
                var notification = dc.Notifications.FirstOrDefault(x => !x.IsDeleted && x.SmartBoardID == notificationSmartboardID);
                if (notification == null)
                {
                    return false;
                }

                //check if the notification is a propertyNote
                if (notification.Value.HasValue)
                {
                    var propertyNote = dc.PropertyNotes.Include(x => x.Property).Include(x => x.Property.Territory).FirstOrDefault(x => x.Guid == notification.Value);
                    if (propertyNote != null && propertyNote?.Property != null)
                    {
                        //validate the token
                        var sbSettings = _ouSettingService
                                            .Value
                                            .GetOUSettingForPropertyID<ICollection<SelectedIntegrationOption>>(propertyNote.Property.Guid, SolarTrackerResources.SelectedSettingName)?
                                            .FirstOrDefault(s => s.Data?.SMARTBoard != null)?
                                            .Data?
                                            .SMARTBoard;

                        if (sbSettings?.ApiKey != apiKey)
                        {
                            throw new Exception("ApiKey is not found");
                        }


                        notification.Status = IgniteNotificationSeenStatus.Seen;
                        notification.SeenAt = DateTime.UtcNow;


                        dc.SaveChanges();
                    }

                }

                return true;
            }
        }

        public async Task<bool> SendPushNotificationAsync(string message, string device, string title)
        {
            try
            {
                string ServerKey = ConfigurationManager.AppSettings["Firebase.ServerKey"];
                var request = new HttpRequestMessage(HttpMethod.Post, "https://fcm.googleapis.com/fcm/send");
                request.Headers.TryAddWithoutValidation("Authorization", "key=" + ServerKey);

                string json = "{\"to\": [" + device + "],\"data\": {\"title\": \"" + title + "\",\"body\": \"" + message + "\"}}";
                request.Content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage result;

                using (var client = new HttpClient())
                {
                    result = await client.SendAsync(request);
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}

