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
using DataReef.Core;
using System.Data.SqlClient;
using DataReef.TM.Models.Enums;
using Newtonsoft.Json;

namespace DataReef.TM.Services.Services
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(AddressFilterMode = AddressFilterMode.Any)]
    public class PropertyNoteService : DataService<PropertyNote>, IPropertyNoteService
    {
        private readonly Lazy<IOUSettingService> _ouSettingService;
        private readonly Lazy<IOUService> _ouService;
        private readonly Lazy<IAssignmentService> _assignmentService;
        private readonly Lazy<IUserInvitationService> _userInvitationService;
        private readonly Lazy<IPersonService> _personService;
        private readonly Lazy<ISolarSalesTrackerAdapter> _sbAdapter;
        private readonly Lazy<IPushNotificationService> _pushNotificationService;
        private readonly IApiLoggingService _apiLoggingService;
        private readonly Lazy<IRepository> _repository;


        public PropertyNoteService(
            ILogger logger,
            Func<IUnitOfWork> unitOfWorkFactory,
            Lazy<IOUSettingService> ouSettingService,
            Lazy<IOUService> ouService,
            Lazy<IRepository> repository,
            Lazy<IAssignmentService> assignmentService,
            Lazy<IUserInvitationService> userInvitationService,
            Lazy<IPersonService> personService,
            Lazy<IPushNotificationService> pushNotificationService,
            Lazy<ISolarSalesTrackerAdapter> sbAdapter,
            IApiLoggingService apiLoggingService) : base(logger, unitOfWorkFactory)
        {
            _ouSettingService = ouSettingService;
            _ouService = ouService;
            _assignmentService = assignmentService;
            _userInvitationService = userInvitationService;
            _personService = personService;
            _pushNotificationService = pushNotificationService;
            _sbAdapter = sbAdapter;
            _repository = repository;
            _apiLoggingService = apiLoggingService;
        }


        public IEnumerable<PropertyNote> GetNotesByPropertyID(Guid propertyID)
        {
            using (var dc = new DataContext())
            {
                //get property along with the notes
                var notesList = dc
                    .PropertyNotes.Where(p => p.PropertyID == propertyID && !p.IsDeleted)
                    .OrderByDescending(p => p.DateCreated)
                    .ToList();

                return notesList ?? new List<PropertyNote>();
            }
        }

        public IEnumerable<Person> QueryForPerson(Guid propertyID, string email, string name)
        {
            using (var dc = new DataContext())
            {
                var property = dc
                    .Properties
                    .Include(x => x.Territory)
                    .FirstOrDefault(x => !x.IsDeleted && !x.IsArchive && x.Guid == propertyID);
                if (property?.Territory?.OUID == null)
                {
                    return new List<Person>();
                }

                return _ouService.Value.GetPersonsAssociatedWithOUOrAncestor(property.Territory.OUID, email, name);
            }

        }

        public override PropertyNote Insert(PropertyNote entity)
        {
            using (var dc = new DataContext())
            {
                dc.PropertyNotes.Add(entity);
                entity.CreatedByID = entity.CreatedByID ?? SmartPrincipal.UserId;
                dc.SaveChanges();
                entity.SaveResult = SaveResult.SuccessfulInsert;

                var people = dc.People.Where(x => x.Guid == SmartPrincipal.UserId).FirstOrDefault();
                if (entity.ContentType == "Comment")
                {
                    var not = dc.PropertyNotes.Where(x => x.Guid == entity.ParentID).FirstOrDefault();
                    var proprty = dc.Properties.Include(x => x.Territory).FirstOrDefault(x => x.Guid == not.PropertyID);

                    not.Updated(SmartPrincipal.UserId, people?.Name);
                    dc.SaveChanges();

                    if (not != null && proprty != null)
                    {
                        NotifyComment(not.PersonID, not, proprty, dc);
                    }
                }

                var property = dc.Properties.Include(x => x.Territory).FirstOrDefault(x => x.Guid == entity.PropertyID);

                if (property != null)
                {
                    //send notifications to the tagged users
                    var taggedPersons = GetTaggedPersons(entity.Content);
                    if (taggedPersons?.Any() == true)
                    {
                        var emails = taggedPersons?.Select(x => x.EmailAddressString);
                        var taggedPersonIds = taggedPersons.Select(x => x.Guid);
                        VerifyUserAssignmentsAndInvite(taggedPersonIds, property, true, null);
                        if (emails?.Any() == true)
                        {
                            SendEmailNotification(entity.Content, entity.CreatedByName, emails, property, entity.Guid);
                        }

                        NotifyTaggedUsers(taggedPersons, entity, property, dc);
                    }

                }

            }
            return entity;
        }

        public override PropertyNote Insert(PropertyNote entity, DataContext dataContext)
        {

            var ret = base.Insert(entity, dataContext);

            using (var dc = new DataContext())
            {
                var people = dc.People.Where(x => x.Guid == SmartPrincipal.UserId).FirstOrDefault();

                if (entity.ContentType == "Comment")
                {
                    var not = dc.PropertyNotes.Where(x => x.Guid == entity.ParentID).FirstOrDefault();
                    var proprty = dc.Properties.Include(x => x.Territory).FirstOrDefault(x => x.Guid == not.PropertyID);

                    not.Updated(SmartPrincipal.UserId, people?.Name);
                    dc.SaveChanges();

                    if (not != null && proprty != null)
                    {
                        NotifyComment(not.PersonID, not, proprty, dc);
                    }
                }

                var property = dc.Properties.Include(x => x.Territory).FirstOrDefault(x => x.Guid == entity.PropertyID);

                if (property != null)
                {
                    //send notifications to the tagged users
                    var taggedPersons = GetTaggedPersons(entity.Content);
                    if (taggedPersons?.Any() == true)
                    {
                        var emails = taggedPersons?.Select(x => x.EmailAddressString);
                        var taggedPersonIds = taggedPersons.Select(x => x.Guid);
                        VerifyUserAssignmentsAndInvite(taggedPersonIds, property, true, null);
                        if (emails?.Any() == true)
                        {
                            SendEmailNotification(entity.Content, entity.CreatedByName, emails, property, entity.Guid);
                        }

                        NotifyTaggedUsers(taggedPersons, entity, property, dc);
                    }

                }

            }
            return ret;
        }

        public override ICollection<PropertyNote> InsertMany(ICollection<PropertyNote> entities)
        {
            var ret = base.InsertMany(entities);

            using (var dc = new DataContext())
            {
                var propertyIds = entities.Select(x => x.PropertyID);

                var properties = dc.Properties.Include(x => x.Territory).Where(x => propertyIds.Contains(x.Guid)).ToList();
                foreach (var entity in entities)
                {
                    var people = dc.People.Where(x => x.Guid == SmartPrincipal.UserId).FirstOrDefault();

                    if (entity.ContentType == "Comment")
                    {
                        var not = dc.PropertyNotes.Where(x => x.Guid == entity.ParentID).FirstOrDefault();
                        var proprty = dc.Properties.Include(x => x.Territory).FirstOrDefault(x => x.Guid == not.PropertyID);

                        not.Updated(SmartPrincipal.UserId, people?.Name);
                        dc.SaveChanges();

                        if (not != null && proprty != null)
                        {
                            NotifyComment(not.PersonID, not, proprty, dc);
                        }
                    }

                    var property = properties.FirstOrDefault(p => p.Guid == entity.PropertyID);
                    if (property != null)
                    {
                        //send notifications to the tagged users
                        var taggedPersons = GetTaggedPersons(entity.Content);
                        if (taggedPersons?.Any() == true)
                        {
                            var emails = taggedPersons?.Select(x => x.EmailAddressString);
                            var taggedPersonIds = taggedPersons.Select(x => x.Guid);
                            VerifyUserAssignmentsAndInvite(taggedPersonIds, property, true, null);
                            if (emails?.Any() == true)
                            {
                                SendEmailNotification(entity.Content, entity.CreatedByName, emails, property, entity.Guid);
                            }

                            NotifyTaggedUsers(taggedPersons, entity, property, dc);
                        }

                    }


                }
            }

            return ret;

        }

        //public override PropertyNote Update(PropertyNote entity)
        //{
        //    var ret = base.Update(entity);
        //    using (var dc = new DataContext())
        //    {
        //        var property = dc.Properties.Include(x => x.Territory).FirstOrDefault(x => x.Guid == entity.PropertyID);

        //        if (property != null)
        //        {
        //            //send notifications to the tagged users
        //            var taggedPersons = GetTaggedPersons(entity.Content);
        //            if(taggedPersons?.Any() == true)
        //            {
        //                var emails = taggedPersons?.Select(x => x.EmailAddressString);
        //                var taggedPersonIds = taggedPersons.Select(x => x.Guid);
        //                VerifyUserAssignmentsAndInvite(taggedPersonIds, property);
        //                if (emails?.Any() == true)
        //                {
        //                    SendEmailNotification(entity.Content, emails, property);
        //                }

        //                NotifyTaggedUsers(taggedPersons, entity.Guid, property.Name, dc);
        //            }

        //        }
        //    }
        //    return ret;
        //}

        public override PropertyNote Update(PropertyNote entity, DataContext dataContext)
        {
            var ret = base.Update(entity, dataContext);

            using (var dc = new DataContext())
            {
                var people = dc.People.Where(x => x.Guid == SmartPrincipal.UserId).FirstOrDefault();

                if (entity.ContentType == "Comment")
                {
                    var not = dc.PropertyNotes.Where(x => x.Guid == entity.ParentID).FirstOrDefault();
                    var proprty = dc.Properties.Include(x => x.Territory).FirstOrDefault(x => x.Guid == not.PropertyID);

                    not.Updated(SmartPrincipal.UserId, people?.Name);
                    dc.SaveChanges();

                    if (not != null && proprty != null)
                    {
                        NotifyComment(not.PersonID, not, proprty, dc);
                    }
                }

                var property = dc.Properties.Include(x => x.Territory).FirstOrDefault(x => x.Guid == entity.PropertyID);

                if (property != null)
                {
                    //send notifications to the tagged users
                    var taggedPersons = GetTaggedPersons(entity.Content);
                    if (taggedPersons?.Any() == true)
                    {
                        var emails = taggedPersons?.Select(x => x.EmailAddressString);
                        var taggedPersonIds = taggedPersons.Select(x => x.Guid);
                        VerifyUserAssignmentsAndInvite(taggedPersonIds, property, true, null);
                        if (emails?.Any() == true)
                        {
                            SendEmailNotification(entity.Content, entity.CreatedByName, emails, property, entity.Guid);
                        }

                        NotifyTaggedUsers(taggedPersons, entity, property, dc);
                    }
                }


            }
            return ret;
        }

        public override ICollection<PropertyNote> UpdateMany(ICollection<PropertyNote> entities)
        {
            var ret = base.UpdateMany(entities);

            using (var dc = new DataContext())
            {
                var propertyIds = entities.Select(x => x.PropertyID);

                var properties = dc.Properties.Include(x => x.Territory).Where(x => propertyIds.Contains(x.Guid)).ToList();
                foreach (var entity in entities)
                {

                    var people = dc.People.Where(x => x.Guid == SmartPrincipal.UserId).FirstOrDefault();

                    if (entity.ContentType == "Comment")
                    {
                        var not = dc.PropertyNotes.Where(x => x.Guid == entity.ParentID).FirstOrDefault();
                        var proprty = dc.Properties.Include(x => x.Territory).FirstOrDefault(x => x.Guid == not.PropertyID);

                        not.Updated(SmartPrincipal.UserId, people?.Name);
                        dc.SaveChanges();

                        if (not != null && proprty != null)
                        {
                            NotifyComment(not.PersonID, not, proprty, dc);
                        }
                    }

                    var property = properties.FirstOrDefault(p => p.Guid == entity.PropertyID);
                    if (property != null)
                    {
                        //send notifications to the tagged users
                        var taggedPersons = GetTaggedPersons(entity.Content);
                        if (taggedPersons?.Any() == true)
                        {
                            var emails = taggedPersons?.Select(x => x.EmailAddressString);
                            var taggedPersonIds = taggedPersons.Select(x => x.Guid);
                            VerifyUserAssignmentsAndInvite(taggedPersonIds, property, true, null);
                            if (emails?.Any() == true)
                            {
                                SendEmailNotification(entity.Content, entity.CreatedByName, emails, property, entity.Guid);
                            }

                            NotifyTaggedUsers(taggedPersons, entity, property, dc);
                        }

                    }


                }
            }

            return ret;
        }

        public IEnumerable<SBNoteDTO> GetAllNotesForProperty(long? smartboardLeadID, long? igniteID, string apiKey)
        {
            using (var dc = new DataContext())
            {
                //first get the property
                var property = GetPropertyAndValidateToken(smartboardLeadID, igniteID, apiKey);
                var userIds = property?.PropertyNotes?.Select(x => x.PersonID) ?? new List<Guid>();

                var users = dc.People.Where(x => !x.IsDeleted && userIds.Contains(x.Guid)).ToList();


                return property.PropertyNotes?.Select(x => new SBNoteDTO
                {
                    Guid = x.Guid,
                    PropertyID = property.Guid,
                    LeadID = property.SmartBoardId,
                    IgniteID = property.Id,
                    Content = x.Content,
                    DateCreated = x.DateCreated,
                    DateLastModified = x.DateLastModified,
                    UserID = users?.FirstOrDefault(u => u.Guid == x.PersonID)?.SmartBoardID,
                    ContentType = x.ContentType,
                    ParentID = x.ParentID
                });
            }
        }

        public SBNoteDTO AddNoteFromSmartboard(SBNoteDTO noteRequest, string apiKey)
        {
            using (var dc = new DataContext())
            {
                //first get the property
                if (!noteRequest.LeadID.HasValue || !noteRequest.IgniteID.HasValue)
                {
                    throw new Exception("LeadID or IgniteID is required");
                }
                var property = GetPropertyAndValidateToken(noteRequest.LeadID, noteRequest.IgniteID, apiKey);

                //get user by the the smartboardId
                //var user = dc.People.FirstOrDefault(x => !x.IsDeleted && (noteRequest.UserID != null &&  x.SmartBoardID.Equals(noteRequest.UserID, StringComparison.InvariantCultureIgnoreCase)
                //                                            || (noteRequest.Email != null && x.EmailAddressString.Equals(noteRequest.Email))));

                var user = dc.People.FirstOrDefault(x => !x.IsDeleted
                                                      && (x.SmartBoardID.Equals(noteRequest.UserID, StringComparison.InvariantCultureIgnoreCase)
                                                            || (noteRequest.Email != null && x.EmailAddressString.Equals(noteRequest.Email))));
                if (user == null)
                {
                    throw new Exception("User with the specified ID was not found");
                }

                var note = new PropertyNote
                {
                    CreatedByID = user.Guid,
                    CreatedByName = user.FullName,
                    PersonID = user.Guid,
                    Content = noteRequest.Content,
                    PropertyID = property.Guid
                };

                dc.PropertyNotes.Add(note);
                dc.SaveChanges();

                //if reply type is comment
                if (noteRequest.ContentType == "Comment")
                {
                    var not = dc.PropertyNotes.Where(x => x.Guid == noteRequest.ParentID).FirstOrDefault();

                    not.Updated(user.Guid, user?.Name);
                    dc.SaveChanges();

                    if (not != null && property != null)
                    {
                        NotifyComment(not.PersonID, not, property, dc);
                    }
                }

                //send notifications to the tagged users
                var taggedPersons = GetTaggedPersons(note.Content);
                if (taggedPersons?.Any() == true)
                {
                    var emails = taggedPersons?.Select(x => x.EmailAddressString);
                    var taggedPersonIds = taggedPersons.Select(x => x.Guid);
                    VerifyUserAssignmentsAndInvite(taggedPersonIds, property, true, user.Guid);
                    if (emails?.Any() == true)
                    {
                        SendEmailNotification(note.Content, note.CreatedByName, emails, property, note.Guid);
                    }

                    NotifyTaggedUsers(taggedPersons, note, property, dc);
                }


                return new SBNoteDTO
                {
                    Guid = note.Guid,
                    PropertyID = property.Guid,
                    LeadID = property.SmartBoardId,
                    Content = note.Content,
                    DateCreated = note.DateCreated,
                    DateLastModified = note.DateLastModified,
                    UserID = user.SmartBoardID,
                    Email = user.EmailAddressString,
                    ContentType = noteRequest.ContentType,
                    ParentID = noteRequest.ParentID
                };
            }
        }

        public SBNoteDTO EditNoteFromSmartboard(SBNoteDTO noteRequest, string apiKey)
        {
            using (var dc = new DataContext())
            {
                if (!noteRequest.Guid.HasValue)
                {
                    throw new Exception("The note Guid is required");
                }

                //get the note
                var note = dc
                    .PropertyNotes
                    .Include(x => x.Property)
                    .Include(x => x.Person)
                    .FirstOrDefault(x => x.Guid == noteRequest.Guid);
                if (note == null)
                {
                    throw new Exception("The note with the specified Guid was not found");
                }

                //get the property
                var property = GetPropertyAndValidateToken(note.Property?.SmartBoardId, note.Property?.Id, apiKey);
                if (property == null)
                {
                    throw new Exception("The lead was not found");
                }

                //get user by the the smartboardId
                var smartboardUserID = noteRequest.UserID;
                Person user = null;
                if (!string.IsNullOrEmpty(noteRequest.UserID) || !string.IsNullOrEmpty(noteRequest.Email))
                {
                    user = dc.People.FirstOrDefault(x => !x.IsDeleted
                                                           && (x.SmartBoardID.Equals(noteRequest.UserID, StringComparison.InvariantCultureIgnoreCase)
                                                            || (noteRequest.Email != null && x.EmailAddressString.Equals(noteRequest.Email, StringComparison.InvariantCultureIgnoreCase))));
                    if (user == null)
                    {
                        throw new Exception("User with the specified ID was not found");
                    }
                    note.Updated(user?.Guid, user.Name);
                }
                else
                {
                    smartboardUserID = note.Person?.SmartBoardID;
                }

                note.Content = noteRequest.Content;

                //if reply type is comment
                if (noteRequest.ContentType == "Comment")
                {
                    var not = dc.PropertyNotes.Where(x => x.Guid == noteRequest.ParentID).FirstOrDefault();

                    not.Updated(user.Guid, user?.Name);
                    dc.SaveChanges();

                    if (not != null && property != null)
                    {
                        NotifyComment(not.PersonID, not, property, dc);
                    }
                }

                var taggedPersons = GetTaggedPersons(note.Content);
                if (taggedPersons?.Any() == true)
                {
                    var emails = taggedPersons?.Select(x => x.EmailAddressString);
                    var taggedPersonIds = taggedPersons.Select(x => x.Guid);
                    VerifyUserAssignmentsAndInvite(taggedPersonIds, property, true, user.Guid);
                    if (emails?.Any() == true)
                    {
                        SendEmailNotification(note.Content, note.CreatedByName, emails, property, note.Guid);
                    }

                    NotifyTaggedUsers(taggedPersons, note, property, dc);
                }



                dc.SaveChanges();

                return new SBNoteDTO
                {
                    Guid = note.Guid,
                    PropertyID = property.Guid,
                    LeadID = property.SmartBoardId,
                    Email = user.EmailAddressString,
                    Content = note.Content,
                    DateCreated = note.DateCreated,
                    DateLastModified = note.DateLastModified,
                    UserID = smartboardUserID,
                    ContentType = noteRequest.ContentType,
                    ParentID = noteRequest.ParentID
                };
            }
        }


        public SBNoteDTO DeleteNoteFromSmartboard(Guid noteID, string userID, string apiKey, string email)
        {
            using (var dc = new DataContext())
            {
                //get the note
                var note = dc.PropertyNotes.Include(x => x.Property).FirstOrDefault(x => x.Guid == noteID);
                if (note == null)
                {
                    throw new Exception("The note with the specified Guid was not found");
                }

                //get the user who deleted the note
                var user = dc.People.FirstOrDefault(x => !x.IsDeleted
                                               && (x.SmartBoardID.Equals(userID, StringComparison.InvariantCultureIgnoreCase) || x.EmailAddressString.Equals(email, StringComparison.InvariantCultureIgnoreCase)));
                if (user == null)
                {
                    throw new Exception("No user found with the specified ID");
                }

                //get the property
                var property = GetPropertyAndValidateToken(note.Property?.SmartBoardId, note.Property?.Id, apiKey);

                note.IsDeleted = true;
                note.Updated(user.Guid);

                dc.SaveChanges();

                return new SBNoteDTO
                {
                    Action = "Delete",
                    Guid = note.Guid,
                    PropertyID = property.Guid,
                    LeadID = property.SmartBoardId,
                    Email = user.EmailAddressString,
                    Content = note.Content,
                    DateCreated = note.DateCreated,
                    DateLastModified = note.DateLastModified,
                    UserID = user.SmartBoardID
                };

            }
        }

        public IEnumerable<SBNoteData> NotesCreate(NoteCreateDTO noteRequest, DateTime fromDate, DateTime toDate)
        {
            try
            {
                string userID = string.Join(",", noteRequest.userId);
                string apiKey = string.Join(",", noteRequest.apiKey);

                using (var dc = new DataContext())
                {

                    var NoteList = dc
                        .Database
                        .SqlQuery<SBNoteData>("exec usp_getnoteDatagroupbyProperty @fromdate, @todate, @apiKey, @userid",
                        new SqlParameter("@fromdate", fromDate),
                        new SqlParameter("@todate", toDate),
                        new SqlParameter("@apiKey", apiKey),
                        new SqlParameter("@userid", userID))
                        .ToList();


                    NoteList.RemoveAll(x => x.LeadID == null || x.apiKey == null || x.DateCreated == null);

                    return NoteList;

                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        public SBUpdateProperty UpdateTerritoryIdInProperty(long? leadId, Guid? TerritoryId, string apiKey, string email)
        {
            using (var dc = new DataContext())
            {
                //get the note
                var propertye = dc.Properties.Where(x => x.SmartBoardId == leadId).FirstOrDefault();
                if (propertye == null)
                {
                    throw new Exception("The Lead with the specified LeadId was not found");
                }

                if (TerritoryId == null)
                {
                    throw new Exception("Please select Territory");
                }

                var territory = dc.Territories.Where(x => x.Guid == TerritoryId).FirstOrDefault();
                if (territory == null)
                {
                    throw new Exception("The Territory with the specified TerritoryId was not found");
                }

                //get the user who transfered the Lead Territory
                var user = dc.People.FirstOrDefault(x => !x.IsDeleted
                                               && (x.EmailAddressString.Equals(email)));
                if (user == null)
                {
                    throw new Exception("No user found with the specified ID");
                }

                propertye.TerritoryID = territory.Guid;
                propertye.Updated(user.Guid);
                dc.SaveChanges();

                //get the property
                var property = GetPropertyAndValidateToken(propertye?.SmartBoardId, propertye?.Id, apiKey);

                return new SBUpdateProperty
                {
                    LeadId = property.SmartBoardId,
                    PropertyId = property.Guid,
                    apiKey = apiKey
                };


            }
        }


        private void NotifyTaggedUsers(IEnumerable<Person> people, PropertyNote note, Property property, DataContext dataContext = null)
        {
            if (people?.Any() != true)
            {
                return;
            }
            bool releaseDataContext = dataContext == null;
            dataContext = dataContext ?? new DataContext();

            var notifications = new List<Notification>();
            foreach (var person in people.Distinct())
            {
                string smartboardNotificationID = null;
                try
                {
                    smartboardNotificationID = _sbAdapter.Value.AddUserTaggingNotification(property, note.CreatedByID.Value, person.Guid);
                    smartboardNotificationID = smartboardNotificationID.Replace("\"", string.Empty);
                }
                catch { }

                notifications.Add(new Notification
                {
                    Value = note.Guid,
                    Description = $"",
                    PersonID = person.Guid,
                    Content = $"New activity on note for property {property.Name}",
                    SmartBoardID = smartboardNotificationID,
                    CreatedByID = note.CreatedByID,
                    CreatedByName = note.CreatedByName
                });

                //send notification

                Notification notification = new Notification();
                notification.NoteID = note.Guid;
                notification.PropertyID = note.PropertyID;

                if (!String.IsNullOrEmpty(person?.fcm_token))
                {
                    _pushNotificationService.Value.PushNotification("You received new notes", person.fcm_token, "Ignite", notification, "Notes");
                }
            }


            if (notifications?.Any() == true)
            {
                dataContext.Notifications.AddRange(notifications);
                dataContext.SaveChanges();
            }

            if (releaseDataContext)
            {
                dataContext.Dispose();
            }

        }

        private IEnumerable<Person> GetTaggedPersons(string content)
        {
            var emailTagPattern = @"\[email:'(.*?)'\](.*?)\[\/email]";
            var regex = new Regex(emailTagPattern);

            var matches = regex.Matches(content);

            var emails = new List<string>();


            if (matches != null)
            {
                foreach (Match m in matches)
                {
                    if (m.Groups.Count == 3)
                    {
                        emails.Add(m.Groups[1].Value);
                    }
                }
            }

            using (var dc = new DataContext())
            {
                return dc.People.Where(x => !x.IsDeleted && emails.Contains(x.EmailAddressString)).ToList();
            }
        }


        private Property GetPropertyAndValidateToken(long? smartboardLeadID, long? igniteID, string apiKey)
        {
            using (var dc = new DataContext())
            {
                if (!smartboardLeadID.HasValue)
                {
                    return null;
                }
                //first get the property
                var property = dc.Properties
                    .Include(x => x.Territory)
                    .Include(x => x.PropertyNotes)
                    .FirstOrDefault(x => x.SmartBoardId == smartboardLeadID || x.Id == igniteID);

                if (property == null)
                {
                    throw new Exception("No lead found with the specified ID(s)");
                }
                property.PropertyNotes = property.PropertyNotes?.Where(p => !p.IsDeleted)?.ToList();
                //validate the token
                var sbSettings = _ouSettingService
                                    .Value
                                    .GetOUSettingForPropertyID<ICollection<SelectedIntegrationOption>>(property.Guid, SolarTrackerResources.SelectedSettingName)?
                                    .FirstOrDefault(s => s.Data?.SMARTBoard != null)?
                                    .Data?
                                    .SMARTBoard;

                if (sbSettings?.ApiKey != apiKey)
                {
                    throw new Exception("Please send Valid Apikey base on LeadId.");
                }

                return property;
            }

        }

        public string getApiKey(long? smartboardLeadID, long? igniteID, string apiKey)
        {

            using (var dc = new DataContext())
            {
                if (!smartboardLeadID.HasValue)
                {
                    return null;
                }
                //first get the property
                var property = dc.Properties
                    .Include(x => x.Territory)
                    .Include(x => x.PropertyNotes)
                    .FirstOrDefault(x => x.SmartBoardId == smartboardLeadID || x.Id == igniteID);

                if (property == null)
                {
                    throw new Exception("No lead found with the specified ID(s)");
                }
                //  property.PropertyNotes = property.PropertyNotes?.Where(p => !p.IsDeleted)?.ToList();
                //validate the token
                var sbSettings = _ouSettingService
                                    .Value
                                    .GetOUSettingForPropertyID<ICollection<SelectedIntegrationOption>>(property.Guid, SolarTrackerResources.SelectedSettingName)?
                                    .FirstOrDefault(s => s.Data?.SMARTBoard != null)?
                                    .Data?
                                    .SMARTBoard;

                return sbSettings?.ApiKey;
            }

        }

        public IEnumerable<Territories> GetTerritoriesList(long smartboardLeadID, string apiKey)
        {
            using (var dc = new DataContext())
            {
                //first get the property
                var property = dc.Properties.FirstOrDefault(x => x.SmartBoardId == smartboardLeadID);

                if (property == null)
                {
                    throw new Exception("No lead found with the specified ID(s)");
                }

                //-- exec usp_GetTerritoryIdsNameByapiKey 29.973433, -95.243265, '1f82605d3fe666478f3f4f1ee25ae828'
                var TerritoriesList = dc
             .Database
             .SqlQuery<Territories>("exec usp_GetTerritoryIdsNameByapiKey @latitude, @longitude, @apiKey", new SqlParameter("@latitude", property.Latitude), new SqlParameter("@longitude", property.Longitude), new SqlParameter("@apiKey", apiKey))
             .ToList();

                return TerritoriesList;
            }
        }


        #region check latitude- longitude are available in polygon(wellknownText) region or not 
        //// var ij = _ouService.IsInside(request.Request, -95.243265, 29.973433);
        //public bool IsInside(string wkt, double longitude, double latitude)
        //{
        //    DbGeography point = DbGeography.FromText(string.Format("POINT({1} {0})", latitude.ToString().Replace(',', '.'), longitude.ToString().Replace(',', '.')), DbGeography.DefaultCoordinateSystemId);

        //    DbGeography polygon = DbGeography.FromText(wkt);
        //    var wellKnownText = polygon.AsText();

        //    var sqlGeography =
        //        Microsoft.SqlServer.Types.SqlGeography.STGeomFromText(new System.Data.SqlTypes.SqlChars(wellKnownText), DbGeography.DefaultCoordinateSystemId)
        //            .MakeValid();

        //    //Now get the inversion of the above area
        //    var invertedSqlGeography = sqlGeography.ReorientObject();

        //    //Whichever of these is smaller is the enclosed polygon, so we use that one.
        //    if (sqlGeography.STArea() > invertedSqlGeography.STArea())
        //    {
        //        sqlGeography = invertedSqlGeography;
        //    }

        //    polygon = DbSpatialServices.Default.GeographyFromProviderValue(sqlGeography);

        //    return point.Intersects(polygon);
        //}
        #endregion check latitude- longitude are available in polygon(wellknownText) region or not 



        //private void SendEmailNotification(string content, IEnumerable<string> emails, Property property, Guid noteID)
        //{
        //    Task.Factory.StartNew(() =>
        //    {
        //        emails = emails.Distinct();
        //        var tag1Regex = new Regex(@"\[email:'(.*?)'\]");
        //        var tag2Regex = new Regex(@"\[\/email\]");

        //        content = tag1Regex.Replace(content, "<b>");
        //        content = tag2Regex.Replace(content, "</b>");

        //        var directNoteLinks = $"<a href='{Constants.CustomURL}notes?propertyID={property.Guid}&noteID={noteID}'>Click here to open the note directly in IGNITE (Link only works on iOS devices)</a><br/> <a href='{Constants.SmartboardURL}/leads/view/{property.SmartBoardId}?showNote=1&note_id={noteID}'>Click here to open the note directly in SmartBoard</a>";
        //        var body = $"New activity has been recorded on a note you were tagged in. <br/> The note is for {property.Name} at {property.Address1} {property.City}, {property.State}. <br/> Here's the note content: <br/><br/> {content} <br/><br/>{directNoteLinks}";
        //        var to = string.Join(";", emails);

        //        Mail.Library.SendEmail(to, string.Empty, $"New note for {property.Name} at {property.Address1} {property.City}, {property.State}", body, true);
        //    });
        //}



        private void SendEmailNotification(string content, string Username, IEnumerable<string> emails, Property property, Guid noteID)
        {
            Task.Factory.StartNew(() =>
            {
                emails = emails.Distinct();
                var tag1Regex = new Regex(@"\[email:'(.*?)'\]");
                var tag2Regex = new Regex(@"\[\/email\]");

                content = tag1Regex.Replace(content, "<b>");
                content = tag2Regex.Replace(content, "</b>");

                //var directNoteLinks = $"<a href='{Constants.CustomURL}notes?propertyID={property.Guid}&noteID={noteID}'>Click here to open the note directly in IGNITE (Link only works on iOS devices)</a><br/> <a href='{Constants.SmartboardURL}/leads/view/{property.SmartBoardId}?showNote=1&note_id={noteID}'>Click here to open the note directly in SMARTBoard</a>";

                var directNoteLinks = $"<a href='{Constants.APIBaseAddress}/home/redirect?notes?propertyID={property.Guid}&noteID={noteID}'>Click here to open the note directly in IGNITE (Link only works on iOS devices)</a><br/> <a href='{Constants.SmartboardURL}/leads/view/{property.SmartBoardId}?showNote=1&note_id={noteID}'>Click here to open the note directly in SMARTBoard</a>";
                //var body = $"Note Sent by: {Username}<br/><br/>New activity has been recorded on a note you were tagged in. <br/> The note is for {property.Name} at {property.Address1} {property.City}, {property.State}. <br/> Here's the note content: <br/><br/> {content} . /*<br/><br/> Note Sent by: {Username}*/<br/><br/><b>Do not Reply</b><br/><br/>{directNoteLinks}";

                var body = $"Note Sent by: {Username}<br/><br/>New activity has been recorded on a note you were tagged in. <br/> The note is for {property.Name} at {property.Address1} {property.City}, {property.State}. <br/> Here's the note content: <br/><br/> {content} . <br/><br/><b>Do not Reply</b><br/><br/>{directNoteLinks}";
                var to = string.Join(";", emails);

                Mail.Library.SendEmail(to, string.Empty, $"New note for {property.Name} at {property.Address1} {property.City}, {property.State}", body, true);
            });
        }

        private void VerifyUserAssignmentsAndInvite(IEnumerable<Guid> userIds, Property property, bool IsFromSmartBoard, Guid? sbuserid)
        {
            if (userIds?.Any() != true)
            {
                return;
            }

            if (property == null)
            {
                return;
            }

            using (var dc = new DataContext())
            {
                var persons = dc.People
                    .Include(x => x.Assignments)
                    .Where(x => !x.IsDeleted && userIds.Contains(x.Guid))
                    .ToList();

                if (persons.Any() == true)
                {

                    foreach (var person in persons)
                    {
                        //check if the user is assigned to the territory
                        if (person.Assignments.Any(a => !a.IsDeleted && a.TerritoryID == property.TerritoryID) != true)
                        {
                            //check if the user is associated with the OU that contains the territory
                            var validations = _assignmentService.Value.ValidatePeopleOUs(new List<KeyValuePair<Guid, Guid>> { new KeyValuePair<Guid, Guid>(person.Guid, property.Territory.OUID) });
                            if (validations.Any())
                            {
                                //user is not associated with the OU. send an invitation
                                var invite = new UserInvitation
                                {
                                    FromPersonID = sbuserid != null ? sbuserid.Value : SmartPrincipal.UserId,
                                    EmailAddress = person.EmailAddressString,
                                    FirstName = person.FirstName,
                                    LastName = person.LastName,
                                    OUID = property.Territory.OUID,
                                    RoleID = OURole.MemberRoleID,
                                    CreatedByID = sbuserid != null ? sbuserid : SmartPrincipal.UserId
                                };
                                _userInvitationService.Value.Insert(invite);
                            }

                            //assign user to the territory
                            _assignmentService.Value.Insert(new Assignment
                            {
                                CreatedByID = SmartPrincipal.UserId,
                                PersonID = person.Guid,
                                TerritoryID = property.TerritoryID,
                                Status = AssignmentStatus.Open,
                                Notes = (IsFromSmartBoard == true) ? "FromNotes" : null
                            });
                        }
                    }
                }
            }


        }


        private void NotifyComment(Guid prsnid, PropertyNote note, Property property, DataContext dataContext = null)
        {
            bool releaseDataContext = dataContext == null;
            dataContext = dataContext ?? new DataContext();

            string smartboardNotificationID = null;
            try
            {
                smartboardNotificationID = _sbAdapter.Value.AddUserTaggingNotification(property, note.CreatedByID.Value, prsnid);
                smartboardNotificationID = smartboardNotificationID.Replace("\"", string.Empty);
            }
            catch { }

            Notification Notification = new Notification
            {
                Value = note.Guid,
                Description = $"",
                PersonID = prsnid,
                Content = $"New Comment on note for property {property.Name}",
                SmartBoardID = smartboardNotificationID,
                CreatedByID = note.CreatedByID,
                CreatedByName = note.CreatedByName
            };

            dataContext.Notifications.Add(Notification);
            dataContext.SaveChanges();

            if (releaseDataContext)
            {
                dataContext.Dispose();
            }

        }

        public string SendNotification(string fcm_token)
        {
            //send notification 

            Notification notification = new Notification();
            notification.NoteID = Guid.Parse("4C3BFF8B-68A5-4D0F-A3B5-CF7E4D377028");
            notification.PropertyID = Guid.Parse("E1CD7CA0-43A7-4A04-AC81-101F51E44C9E");

            var res = _pushNotificationService.Value.PushNotification("You received new notes", fcm_token, "Ignite", notification, "Property");
            return res;
        }

        public string UpdateSmartboardIdByEmail()
        {
            //send notification 
            var response = _sbAdapter.Value.GetAllSbUsers();

            if (response?.users?.Count > 0)
            {
                try
                {

                    //update the user's SmartBoard ID
                    using (var dc = new DataContext())
                    {
                        foreach (var item in response?.users)
                        {
                            var currentPerson = dc.People.FirstOrDefault(x => x.EmailAddressString == item.email);
                            if (currentPerson != null)
                            {
                                if (item.id.ToString() != currentPerson.SmartBoardID)
                                {
                                    currentPerson.SmartBoardID = item.id.ToString();
                                    currentPerson.Updated();

                                    ApiLogEntry apilog = new ApiLogEntry();
                                    apilog.Id = Guid.NewGuid();
                                    apilog.User = SmartPrincipal.UserId.ToString();
                                    apilog.Machine = Environment.MachineName;
                                    apilog.RequestContentType = "Update All Smartboard IDS";
                                    apilog.RequestTimestamp = DateTime.UtcNow;
                                    apilog.RequestUri = "SunnovaCallBackApi";
                                    apilog.ResponseContentBody = "IGNITE-SmartBoardID " + currentPerson.SmartBoardID + "SmartBoardID " + item.id;

                                    dc.ApiLogEntries.Add(apilog);
                                    dc.SaveChanges();
                                }
                            }
                        }

                        dc.SaveChanges();

                    }
                }
                catch (Exception)
                {
                }
            }
            return JsonConvert.SerializeObject(response?.users);
        }
    }
}

