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
                var query = dc.Notifications.Where(x => !x.IsDeleted && x.PersonID == personID);
                if (status.HasValue)
                {
                    query = query.Where(x => x.Status == status.Value);
                }


                return query
                    .GroupBy(x => x.Value)
                    .Select(g => g.OrderByDescending(x => x.DateCreated).FirstOrDefault())
                    .OrderByDescending(x => x.DateCreated)
                    .Skip(pageNumber * itemsPerPage)
                    .Take(itemsPerPage).ToList();
            }
        }

        public int CountUnreadNotifications(Guid personID)
        {
            using (var dc = new DataContext())
            {
                var person = dc.People.Include(x => x.PersonSettings).FirstOrDefault(x => x.Guid == personID);
                if (person == null)
                {
                    return 0;
                }

                var query = dc
                    .Notifications
                    .Where(x => x.PersonID == personID && !x.IsDeleted && x.Status == IgniteNotificationSeenStatus.NotSeen)
                    .GroupBy(x => x.Value)
                    .Select(g => g.OrderByDescending(x => x.DateCreated).FirstOrDefault());

                var lastReadDate = person.PersonSettings.FirstOrDefault(x => x.Name == "Ignite.Notifications.LastChecked");

                if (!string.IsNullOrEmpty(lastReadDate?.Value))
                {
                    if (DateTime.TryParse(lastReadDate?.Value, out var dateLastChecked))
                    {
                        query = query.Where(x => x.DateCreated >= dateLastChecked);
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

        //private RestClient client
        //{
        //    get
        //    {
        //        return new RestClient("https://fcm.googleapis.com/fcm/send");
        //    }
        //} 

        public async Task<string> SendNotificationFromFirebaseCloud(string message, string device, string title)
        {
            try
            {
                //    var msg = new JObject();
                //    msg.Add("to", device);
                //    var notification = new JObject();
                //    notification.Add("title", title);
                //    notification.Add("text", message);
                //    msg.Add("notification", notification); 

                //    string ServerKey = System.Configuration.ConfigurationManager.AppSettings["Firebase.ServerKey"];
                //    var request = new RestRequest(Method.POST); 
                //    request.AddHeader("Authorization", "key=" + ServerKey);
                //    request.AddHeader("Content-Type", "application/json");
                //    request.AddJsonBody(msg);
                //    var response = client.Execute(request);

                //    if (response.StatusCode != HttpStatusCode.OK)
                //    {
                //        throw new ApplicationException($"Notification Failed. {response.Content}");
                //    }

                //    var content = response.Content;

                //string ServerKey = System.Configuration.ConfigurationManager.AppSettings["Firebase.ServerKey"];

                string serverKey = System.Configuration.ConfigurationManager.AppSettings["Firebase.ServerKey"];

                try
                {
                    var result = "-1";
                    var webAddr = "https://fcm.googleapis.com/fcm/send";

                    var regID = device;

                    var httpWebRequest = (HttpWebRequest)WebRequest.Create(webAddr);
                    httpWebRequest.ContentType = "application/json";
                    httpWebRequest.Headers.Add("Authorization" , "key=AAAAjcK0I_g:APA91bE9yx0Ximczoh423GN5fUhOSG5XOYnLxHDJtciBdGcapueC9LhCe0xyMMJwnfY79UrZ83rPXhbQLvW_JOcbbT6xNy_P7U96YKfQXB_U2Zr5Um58Dk0TglI_pvRghEoll5AqfN94");
                    httpWebRequest.Method = "POST";

                    using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                    {
                        string json = "{\"to\": \"" + regID + "\",\"notification\": {\"title\": \"New deal\",\"body\": \"20% deal!\"},\"priority\":10}";
                        //registration_ids, array of strings -  to, single recipient
                        streamWriter.Write(json);
                        streamWriter.Flush();
                    }

                    var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
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

                //var authorizationKey = string.Format("key={0}", ServerKey);
                //HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "https://fcm.googleapis.com/fcm/send");
                //request.Headers.TryAddWithoutValidation("Authorization", authorizationKey);

                //var messageInformation = new
                //{
                //    notification = new
                //    {
                //        title,
                //        body = message
                //    },
                //    to = device
                //};
                //string jsonMessage = JsonConvert.SerializeObject(messageInformation);

                //request.Content = new StringContent(jsonMessage, Encoding.UTF8, "application/json");
                //string result;
                //using (var client = new HttpClient())
                //{
                //    HttpResponseMessage response = await client.SendAsync(request);
                //    result = await response.Content.ReadAsStringAsync();
                //}

                //return result;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}

