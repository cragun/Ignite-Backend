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
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using DataReef.Core.Infrastructure.Authorization;
using DataReef.Core.Classes;
using DataReef.Core;
using System.Data.SqlClient;
using Newtonsoft.Json;
using DataReef.TM.Models.DTOs.FinanceAdapters;
using DataReef.TM.Contracts.Services.FinanceAdapters;
using System.Web.Http;
using System.Net;
using System.Net.Http;
using DataReef.TM.Models.DTOs.Solar.Finance;
using Note = DataReef.TM.Models.DTOs.Solar.Finance.Note;

namespace DataReef.TM.Services.Services
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(AddressFilterMode = AddressFilterMode.Any)]
    public class PropertyNoteService : DataService<PropertyNote>, IPropertyNoteService
    {
        private readonly Lazy<IOUSettingService> _ouSettingService;
        private readonly Lazy<IOUService> _ouService;
        private readonly Lazy<IAuthenticationService> _authService;
        private readonly Lazy<IAssignmentService> _assignmentService;
        private readonly Lazy<IUserInvitationService> _userInvitationService;
        private readonly Lazy<ISolarSalesTrackerAdapter> _sbAdapter;
        private readonly Lazy<IPushNotificationService> _pushNotificationService;
        private readonly Lazy<ISmsService> _smsService;
        private readonly Lazy<IJobNimbusAdapter> _jobNimbusAdapter;
        private readonly Lazy<IPropertyNotesAdapter> _propertyNotesAdapter;

        public PropertyNoteService(
            ILogger logger,
            Func<IUnitOfWork> unitOfWorkFactory,
            Lazy<IOUSettingService> ouSettingService,
            Lazy<IOUService> ouService,
            Lazy<IAuthenticationService> authService,
            Lazy<IAssignmentService> assignmentService,
            Lazy<IUserInvitationService> userInvitationService,
            Lazy<IPushNotificationService> pushNotificationService,
            Lazy<ISolarSalesTrackerAdapter> sbAdapter,
            Lazy<ISmsService> smsService,
              Lazy<IPropertyNotesAdapter> propertyNotesAdapter,
            Lazy<IJobNimbusAdapter> jobNimbusAdapter) : base(logger, unitOfWorkFactory)
        {
            _ouSettingService = ouSettingService;
            _ouService = ouService;
            _authService = authService;
            _assignmentService = assignmentService;
            _userInvitationService = userInvitationService;
            _pushNotificationService = pushNotificationService;
            _sbAdapter = sbAdapter;
            _smsService = smsService;
            _propertyNotesAdapter = propertyNotesAdapter;
            _jobNimbusAdapter = jobNimbusAdapter;
        }


        public async Task<IEnumerable<PropertyNote>> GetNotesByPropertyID(Guid propertyID)
        {
            using (var dc = new DataContext())
            {
                //get property along with the notes
                var notesList = await dc
                    .PropertyNotes.Where(p => p.PropertyID == propertyID && !p.IsDeleted).AsNoTracking()
                    .OrderByDescending(p => p.DateCreated)
                    .ToListAsync();

                return notesList?.Where(a => a.ContentType != "Comment").Select(x => new PropertyNote
                {
                    ParentID = x.ParentID,
                    ContentType = x.ContentType,
                    Attachments = x.Attachments,
                    PropertyType = x.PropertyType,
                    PersonID = x.PersonID,
                    PropertyID = x.PropertyID,
                    Content = x.Content,
                    IsDeleted = x.IsDeleted,
                    DateCreated = x.DateCreated,
                    DateLastModified = x.DateLastModified,
                    CreatedByName = x.CreatedByName,
                    LastModifiedBy = x.LastModifiedBy,
                    LastModifiedByName = x.LastModifiedByName,
                    Replies = notesList?.Where(a => a.ContentType == "Comment" && a.ParentID == x.Guid).ToList(),
                });
            }
        }

        public async Task<IEnumerable<Person>> QueryForPerson(Guid propertyID, string email, string name)
        {
            using (var dc = new DataContext())
            {
                var property = await dc
                    .Properties
                    .Include(x => x.Territory).AsNoTracking()
                    .FirstOrDefaultAsync(x => !x.IsDeleted && !x.IsArchive && x.Guid == propertyID);
                if (property?.Territory?.OUID == null)
                {
                    return new List<Person>();
                }

                return await _ouService.Value.GetPersonsAssociatedWithOUOrAncestor(property.Territory.OUID, email, name);
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

                #region ThirdPartyPropertyType
                if (entity.PropertyType == ThirdPartyPropertyType.Roofing || entity.PropertyType == ThirdPartyPropertyType.Both)
                {
                    _jobNimbusAdapter.Value.CreateJobNimbusNote(entity);
                }
                #endregion ThirdPartyPropertyType 

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

                var users = dc.People.Where(x => !x.IsDeleted && userIds.Contains(x.Guid)).AsNoTracking().ToList();

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
                    Attachments = x.Attachments,
                    ParentID = x.ParentID,
                    Count = property.PropertyNotes?.Count(a => a.ParentID == x.Guid),
                    LastUpdateTime = property.PropertyNotes?.Where(a => a.ParentID == x.Guid).OrderByDescending(a => a.DateLastModified).FirstOrDefault()?.DateLastModified,
                    ContentTags = x.ContentTags
                });
            }

        }

        public IEnumerable<SBNoteDTO> GetNoteComments(long? smartboardLeadID, long? igniteID, string apiKey, Guid ParentID)
        {
            using (var dc = new DataContext())
            {
                //first get the property
                var property = GetPropertyAndValidateToken(smartboardLeadID, igniteID, apiKey);
                var userIds = property?.PropertyNotes?.Select(x => x.PersonID) ?? new List<Guid>();

                var users = dc.People.Where(x => !x.IsDeleted && userIds.Contains(x.Guid)).AsNoTracking().ToList();

                return property.PropertyNotes?.Where(a => a.ParentID == ParentID).Select(x => new SBNoteDTO
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
                    Attachments = x.Attachments,
                    ParentID = x.ParentID,
                    Count = property.PropertyNotes?.Count(a => a.ParentID == x.Guid),
                    LastUpdateTime = property.PropertyNotes?.Where(a => a.ParentID == x.Guid && !a.IsDeleted).OrderByDescending(a => a.DateLastModified).FirstOrDefault()?.DateLastModified,
                    ContentTags = x.ContentTags
                });
            }
        }

        #region transfer notes to new server

        public PropertyNote AddEditNote(PropertyNote entity)
        {
            using (var dc = new DataContext())
            {
                try
                {
                    var people = dc.People.AsNoTracking().FirstOrDefault(x => x.Guid == SmartPrincipal.UserId);

                    if (people == null)
                    {
                        throw new HttpResponseException(new HttpResponseMessage() { StatusCode = HttpStatusCode.NotFound, ReasonPhrase = "User with the specified ID was not found" });
                    }

                    var property = dc.Properties.Include(x => x.Territory).AsNoTracking().FirstOrDefault(x => x.Guid == entity.PropertyID);

                    if (property == null)
                    {
                        throw new HttpResponseException(new HttpResponseMessage() { StatusCode = HttpStatusCode.NotFound, ReasonPhrase = "Property not found" });
                    }

                    entity.DateCreated = DateTime.UtcNow;
                    entity.DateLastModified = DateTime.UtcNow;
                    entity.Guid = Guid.NewGuid();

                    #region ThirdPartyPropertyType

                    if (String.IsNullOrEmpty(entity.NoteID) && entity.PropertyType == ThirdPartyPropertyType.Roofing || entity.PropertyType == ThirdPartyPropertyType.Both)
                    {
                        var response = _jobNimbusAdapter.Value.CreateJobNimbusNote(entity);
                        entity.JobNimbusID = response?.jnid;
                    }

                    #endregion ThirdPartyPropertyType

                    var taggedPersons = GetTaggedPersons(entity.Content);

                    //send notifications to the tagged users 
                    if (taggedPersons.Count() > 0)
                    {
                        var taggedPersonIds = taggedPersons.Select(x => x.Guid);
                        VerifyUserAssignmentsAndInvite(taggedPersonIds, property, true, null);

                        NotifyTaggedUsers(taggedPersons, entity, property, dc);
                    }

                    var parentNote = entity.ParentNote;
                    if (entity.ContentType == "Comment")
                    {
                        if (parentNote == null)
                        {
                            throw new HttpResponseException(new HttpResponseMessage() { StatusCode = HttpStatusCode.NotFound, ReasonPhrase = "Note not found" });
                        }

                        entity.ThreadID = parentNote.ThreadID;

                        var parent = dc.People.AsNoTracking().FirstOrDefault(x => x.Guid == parentNote.PersonID);

                        parentNote.CreatedByID = parent?.Guid;
                        parentNote.CreatedByName = parent?.Name;

                        NotifyComment(parentNote.PersonID, parentNote, property, dc);

                    }

                    var reference = _propertyNotesAdapter.Value.AddEditNote(property.NoteReferenceId, entity, taggedPersons, people);

                    if (entity.ContentType == "Comment")
                    {
                        entity.NoteID = reference?.replyId;
                    }
                    else
                    {
                        entity.NoteID = reference?.noteId;
                    }

                    entity.ThreadID = reference?.threadId;

                    //send email notification to user 
                    var emails = taggedPersons?.Select(x => x.EmailAddressString);
                    if (emails.Count() > 0)
                    {
                        emails = emails.Distinct();

                        string content = entity.Content;

                        var tag1Regex = new Regex(@"\[email:'(.*?)'\]");
                        var tag2Regex = new Regex(@"\[\/email\]");

                        content = tag1Regex.Replace(content, "<b>");
                        content = tag2Regex.Replace(content, "</b>");

                        string noteID = entity.NoteID;
                        if (entity.ContentType == "Comment")
                            noteID = parentNote.NoteID;

                        var directNoteLinks = $"<a href='{Constants.APIBaseAddress}/home/redirect?notes?propertyID={property.Guid}&noteID={entity.Guid}'>Click here to open the note directly in IGNITE (Link only works on iOS devices)</a><br/> <a href='{Constants.SmartboardURL}/leads/viewnote?note_reference_id={property.NoteReferenceId}&thread_id={entity.ThreadID}&note_id={noteID}'>Click here to open the note directly in SMARTBoard</a>";

                        var body = $"Note Sent by: {entity.CreatedByName}<br/><br/>New activity has been recorded on a note you were tagged in. <br/> The note is for {property.Name} at {property.Address1} {property.City}, {property.State}. <br/> Here's the note content: <br/><br/> {entity.Content} . <br/><br/><b>Do not Reply</b><br/><br/>{directNoteLinks}";

                        var to = string.Join(";", emails);

                        _propertyNotesAdapter.Value.SendEmailNotification($"New note for {property.Name} at {property.Address1} {property.City}, {property.State}", body, to);
                    }

                    return entity;
                }
                catch (Exception ex)
                {
                    dc.ApiLogEntries.Add(new ApiLogEntry()
                    {
                        Id = Guid.NewGuid(),
                        User = Convert.ToString(SmartPrincipal.UserId),
                        Machine = Environment.MachineName,
                        RequestContentType = "Add Edit Note",
                        RequestContentBody = JsonConvert.SerializeObject(entity),
                        RequestTimestamp = DateTime.UtcNow,
                        RequestUri = "AddEditNote",
                        ResponseContentBody = ex.Message
                    });
                    dc.SaveChanges();

                    throw new ApplicationException(ex.Message);
                }
            }
        }

        public async Task<IEnumerable<PropertyNote>> GetPropertyNotes(Guid PropertyID)
        {
            using (var dc = new DataContext())
            {
                var property = await dc.Properties.AsNoTracking().FirstOrDefaultAsync(x => x.Guid == PropertyID);

                if (property == null)
                {
                    throw new HttpResponseException(new HttpResponseMessage() { StatusCode = HttpStatusCode.NotFound, ReasonPhrase = "Property not found" });
                }

                var response = await _propertyNotesAdapter.Value.GetPropertyNotes(property.NoteReferenceId);

                List<PropertyNote> noteList = new List<PropertyNote>();

                foreach (var item in response)
                {
                    var note = item.notes;

                    if (note != null)
                    {
                        var data = new PropertyNote
                        {
                            Guid = String.IsNullOrEmpty(note.guid) ? Guid.Empty : Guid.Parse(note.guid),
                            Attachments = String.Join(",", note.attachments, 1),
                            PropertyType = note.propertyType,
                            PersonID = Guid.Parse(note.personId),
                            PropertyID = PropertyID,
                            Content = note.message,
                            DateCreated = Convert.ToDateTime(note.created),
                            DateLastModified = Convert.ToDateTime(note.modified),
                            CreatedByName = await _authService.Value.GetUserName(Guid.Parse(note.personId)),
                            NoteID = note._id,
                            ThreadID = note.threadId,
                            Replies = new List<PropertyNote>()
                        };

                        if (item.replies != null)
                        {
                            foreach (var reply in item.replies)
                            {
                                data.Replies.Add(new PropertyNote
                                {
                                    Guid = String.IsNullOrEmpty(reply.guid) ? Guid.Empty : Guid.Parse(reply.guid),
                                    Attachments = String.Join(",", reply.attachments, 1),
                                    PropertyType = reply.propertyType,
                                    PersonID = Guid.Parse(reply.personId),
                                    PropertyID = PropertyID,
                                    Content = reply.message,
                                    DateCreated = Convert.ToDateTime(reply.created),
                                    DateLastModified = Convert.ToDateTime(reply.modified),
                                    NoteID = note._id,
                                    CreatedByName = await _authService.Value.GetUserName(Guid.Parse(note.personId))
                                });
                            }
                        }

                        noteList.Add(data);
                    }
                }

                return noteList;
            }
        }

        public async Task<PropertyNote> GetPropertyNoteById(Guid NoteID, Guid PropertyID)
        {
            using (var dc = new DataContext())
            {
                var property = await dc.Properties.AsNoTracking().FirstOrDefaultAsync(x => x.Guid == PropertyID);

                if (property == null)
                {
                    throw new HttpResponseException(new HttpResponseMessage() { StatusCode = HttpStatusCode.NotFound, ReasonPhrase = "Property not found" });
                }

                var note = await _propertyNotesAdapter.Value.GetPropertyNoteById(Convert.ToString(NoteID));

                if (note != null)
                {
                    var data = new PropertyNote
                    {
                        Guid = String.IsNullOrEmpty(note.guid) ? Guid.Empty : Guid.Parse(note.guid),
                        Attachments = String.Join(",", note.attachments, 1),
                        PropertyType = note.propertyType,
                        PersonID = Guid.Parse(note.personId),
                        PropertyID = PropertyID,
                        Content = note.message,
                        DateCreated = Convert.ToDateTime(note.created),
                        DateLastModified = Convert.ToDateTime(note.modified),
                        CreatedByName = await _authService.Value.GetUserName(Guid.Parse(note.personId)),
                        NoteID = note._id,
                        ThreadID = note.threadId
                    };

                    return data;
                }
                else
                {
                    return new PropertyNote();
                }
            }
        }

        public string ImportNotes(int limit)
        {
            using (var dc = new DataContext())
            {
                try
                {
                    var properties = dc.Properties.Include(x => x.Territory).Where(a => a.NoteReferenceId == null).OrderByDescending(a => a.DateCreated).Take(limit).ToList();

                    if (properties.Count() == 0)
                    {
                        throw new HttpResponseException(new HttpResponseMessage() { StatusCode = HttpStatusCode.NotFound, ReasonPhrase = "Property not found" });
                    }

                    foreach (var property in properties)
                    {
                        try
                        {
                            var territory = dc.Territories.AsNoTracking().FirstOrDefault(t => !t.IsDeleted && !t.IsArchived && t.Guid == property.TerritoryID);
                            if (territory == null)
                            {
                                throw new HttpResponseException(new HttpResponseMessage() { StatusCode = HttpStatusCode.NotFound, ReasonPhrase = "Territory not found" });
                            }

                            var sbSettings = _ouSettingService
                                         .Value
                                         .GetSettingsByOUID(territory.OUID)
                                         ?.FirstOrDefault(x => x.Name == SolarTrackerResources.SelectedSettingName)
                                         ?.GetValue<ICollection<SelectedIntegrationOption>>()?
                                         .FirstOrDefault(s => s.Data?.SMARTBoard != null)?
                                         .Data?
                                         .SMARTBoard;

                            var reference = _propertyNotesAdapter.Value.GetPropertyReferenceId(property, sbSettings?.ApiKey);
                            property.NoteReferenceId = reference?.refId;
                            property.DateLastModified = DateTime.UtcNow;

                            dc.SaveChanges();

                            if (!String.IsNullOrEmpty(property.NoteReferenceId))
                            {
                                var notesList = dc.PropertyNotes.Where(p => p.PropertyID == property.Guid && !p.IsDeleted).AsNoTracking().ToList();

                                foreach (var note in notesList)
                                {
                                    var taggedPersons = GetTaggedPersons(note.Content);

                                    var people = dc.People.AsNoTracking().FirstOrDefault(x => x.Guid == note.PersonID);

                                    if (people == null)
                                    {
                                        throw new HttpResponseException(new HttpResponseMessage() { StatusCode = HttpStatusCode.NotFound, ReasonPhrase = "User with the specified ID was not found" });
                                    }

                                    var noteReference = _propertyNotesAdapter.Value.AddEditNote(property.NoteReferenceId, note, taggedPersons, people);

                                    note.NoteID = noteReference?.noteId;
                                    note.ThreadID = noteReference?.threadId;

                                    var comments = notesList.Where(a => a.ParentID == note.Guid).ToList();

                                    foreach (var comment in comments)
                                    {
                                        var taggedPersonsComment = GetTaggedPersons(comment.Content);

                                        comment.ThreadID = note.ThreadID;

                                        var peopleComment = dc.People.AsNoTracking().FirstOrDefault(x => x.Guid == comment.PersonID);

                                        if (peopleComment == null)
                                        {
                                            throw new HttpResponseException(new HttpResponseMessage() { StatusCode = HttpStatusCode.NotFound, ReasonPhrase = "User with the specified ID was not found" });
                                        }

                                        var commentReference = _propertyNotesAdapter.Value.AddEditNote(property.NoteReferenceId, comment, taggedPersonsComment, peopleComment);
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            dc.ApiLogEntries.Add(new ApiLogEntry()
                            {
                                Id = Guid.NewGuid(),
                                User = Convert.ToString(SmartPrincipal.UserId),
                                Machine = Environment.MachineName,
                                RequestContentType = "Import Notes",
                                RequestContentBody = JsonConvert.SerializeObject(property),
                                RequestTimestamp = DateTime.UtcNow,
                                RequestUri = "ImportNotes",
                                ResponseContentBody = ex.Message
                            });
                            dc.SaveChanges();
                        }
                    }

                    return "success";
                }
                catch (Exception ex)
                {
                    throw new ApplicationException(ex.Message);
                }
            }
        }

        #endregion

        public SBNoteDTO AddNoteFromSmartboard(SBNoteDTO noteRequest, string apiKey)
        {
            using (var dc = new DataContext())
            {
                //first get the property
                if (!noteRequest.LeadID.HasValue || !noteRequest.IgniteID.HasValue)
                {
                    throw new HttpResponseException(new HttpResponseMessage() { StatusCode = HttpStatusCode.NotFound, ReasonPhrase = "LeadID or IgniteID is required" });
                }
                var property = GetPropertyAndValidateToken(noteRequest.LeadID, noteRequest.IgniteID, apiKey);

                if (property == null)
                {
                    throw new HttpResponseException(new HttpResponseMessage() { StatusCode = HttpStatusCode.NotFound, ReasonPhrase = "The lead was not found" });
                }

                if (property.SmartBoardId == null && noteRequest.LeadID > 0)
                {
                    // var prop = dc.Properties.FirstOrDefault(x => x.Id == property.Id);
                    var prop = dc.Properties.FirstOrDefault(x => x.Id == property.Id);
                    property.SmartBoardId = noteRequest.LeadID;
                    prop.SmartBoardId = noteRequest.LeadID;
                    dc.SaveChanges();
                }

                var user = dc.People.AsNoTracking().FirstOrDefault(x => !x.IsDeleted && (x.SmartBoardID.Equals(noteRequest.UserID, StringComparison.InvariantCultureIgnoreCase) || (noteRequest.Email != null && x.EmailAddressString.Equals(noteRequest.Email))));
                if (user == null)
                {
                    throw new HttpResponseException(new HttpResponseMessage() { StatusCode = HttpStatusCode.NotFound, ReasonPhrase = "User with the specified ID was not found" });
                }

                var note = new PropertyNote
                {
                    CreatedByID = user.Guid,
                    CreatedByName = user.FullName,
                    PersonID = user.Guid,
                    Content = noteRequest.Content,
                    Attachments = noteRequest.Attachments,
                    PropertyID = property.Guid
                };

                Guid emailnoteid = note.Guid;

                //if reply type is comment
                if (noteRequest.ContentType == "Comment")
                {
                    note.ContentType = noteRequest.ContentType;
                    note.ParentID = noteRequest.ParentID;

                    var not = dc.PropertyNotes.Where(x => x.Guid == noteRequest.ParentID).FirstOrDefault();
                    emailnoteid = not.Guid;

                    not.Updated(user.Guid, user?.Name);
                    dc.SaveChanges();

                    if (not != null && property != null)
                    {
                        NotifyComment(not.PersonID, not, property, dc);
                        var personemail = dc.People.Where(x => x.Guid == not.PersonID).FirstOrDefault();

                        if (not.PersonID != user.Guid && !note.Content.Contains(personemail?.EmailAddressString))
                        {
                            if (noteRequest.IsSendEmail)
                            {
                                SendEmailForNotesComment(noteRequest.Content, note.CreatedByName, personemail.EmailAddressString, property, not.Guid, true);
                            }

                            if (noteRequest.IsSendSMS)
                            {
                                string number = personemail?.PhoneNumbers?.FirstOrDefault()?.Number;
                                if (!string.IsNullOrEmpty(number))
                                {
                                    _smsService.Value.SendSms("You received new notes", number);
                                }
                            }
                        }
                    }
                }

                dc.PropertyNotes.Add(note);
                dc.SaveChanges();

                //send notifications to the tagged users
                var taggedPersons = GetTaggedPersons(note.Content);
                if (taggedPersons?.Any() == true)
                {
                    var emails = taggedPersons?.Select(x => x.EmailAddressString);
                    var taggedPersonIds = taggedPersons.Select(x => x.Guid);
                    VerifyUserAssignmentsAndInvite(taggedPersonIds, property, true, user.Guid);
                    NotifyTaggedUsers(taggedPersons, note, property, dc);

                    //email / sms to tagged users
                    if (noteRequest.TaggedUsers?.Count > 0)
                    {
                        List<string> sendemails = new List<string>();
                        foreach (var item in noteRequest.TaggedUsers)
                        {
                            if (item.IsSendEmail)
                            {
                                sendemails.Add(item.email);
                            }

                            if (item.IsSendSMS)
                            {
                                string number = item.PhoneNumber;
                                if (!string.IsNullOrEmpty(number))
                                {
                                    _smsService.Value.SendSms("You received new notes", number);
                                }
                            }
                        }

                        SendEmailNotification(note.Content, note.CreatedByName, sendemails, property, emailnoteid, true);
                    }
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
                    Attachments = noteRequest.Attachments,
                    ParentID = noteRequest.ParentID,
                    TaggedUsers = noteRequest.TaggedUsers,
                    IsSendEmail = noteRequest.IsSendEmail,
                    IsSendSMS = noteRequest.IsSendSMS
                };
            }
        }


        public SBNoteDTO EditNoteFromSmartboard(SBNoteDTO noteRequest, string apiKey)
        {
            using (var dc = new DataContext())
            {
                if (!noteRequest.Guid.HasValue)
                {
                    throw new HttpResponseException(new HttpResponseMessage() { StatusCode = HttpStatusCode.NotFound, ReasonPhrase = "The note Guid is required" });
                }

                //get the note
                var note = dc
                    .PropertyNotes
                    .Include(x => x.Property)
                    .Include(x => x.Person)
                    .FirstOrDefault(x => x.Guid == noteRequest.Guid);
                if (note == null)
                {
                    throw new HttpResponseException(new HttpResponseMessage() { StatusCode = HttpStatusCode.NotFound, ReasonPhrase = "The note with the specified Guid was not found" });
                }

                //get the property
                var property = GetPropertyAndValidateToken(note.Property?.SmartBoardId, note.Property?.Id, apiKey);
                if (property == null)
                {
                    throw new HttpResponseException(new HttpResponseMessage() { StatusCode = HttpStatusCode.NotFound, ReasonPhrase = "The lead was not found" });
                }

                if (property.SmartBoardId == null && noteRequest.LeadID > 0)
                {
                    var prop = dc.Properties.FirstOrDefault(x => x.Id == property.Id);
                    property.SmartBoardId = noteRequest.LeadID;
                    prop.SmartBoardId = noteRequest.LeadID;
                    dc.SaveChanges();
                }

                //get user by the the smartboardId
                var smartboardUserID = noteRequest.UserID;
                if (!String.IsNullOrEmpty(noteRequest.Attachments))
                {
                    note.Attachments = noteRequest.Attachments;
                }

                Person user = null;
                if (!string.IsNullOrEmpty(noteRequest.UserID) || !string.IsNullOrEmpty(noteRequest.Email))
                {
                    user = dc.People.FirstOrDefault(x => !x.IsDeleted
                                                           && (x.SmartBoardID.Equals(noteRequest.UserID, StringComparison.InvariantCultureIgnoreCase)
                                                            || (noteRequest.Email != null && x.EmailAddressString.Equals(noteRequest.Email, StringComparison.InvariantCultureIgnoreCase))));
                    if (user == null)
                    {
                        throw new HttpResponseException(new HttpResponseMessage() { StatusCode = HttpStatusCode.NotFound, ReasonPhrase = "User with the specified ID was not found" });
                    }
                    note.Updated(user?.Guid, user.Name);
                }
                else
                {
                    smartboardUserID = note.Person?.SmartBoardID;
                }

                note.Content = noteRequest.Content;

                Guid emailnoteid = note.Guid;
                //if reply type is comment
                if (noteRequest.ContentType == "Comment")
                {
                    var not = dc.PropertyNotes.Where(x => x.Guid == noteRequest.ParentID).FirstOrDefault();

                    not.Updated(user.Guid, user?.Name);
                    dc.SaveChanges();

                    emailnoteid = not.Guid;

                    if (not != null && property != null)
                    {
                        NotifyComment(not.PersonID, not, property, dc);

                        var personemail = dc.People.Where(x => x.Guid == not.PersonID).FirstOrDefault();

                        if (not.PersonID != user.Guid && !note.Content.Contains(personemail?.EmailAddressString))
                        {
                            if (noteRequest.IsSendEmail)
                            {
                                SendEmailForNotesComment(noteRequest.Content, note.CreatedByName, personemail.EmailAddressString, property, not.Guid, true);
                            }

                            if (noteRequest.IsSendSMS)
                            {
                                string number = personemail?.PhoneNumbers?.FirstOrDefault()?.Number;
                                if (!string.IsNullOrEmpty(number))
                                {
                                    _smsService.Value.SendSms("You received new notes", number);
                                }
                            }
                        }
                    }
                }

                var taggedPersons = GetTaggedPersons(note.Content);
                if (taggedPersons?.Any() == true)
                {
                    var emails = taggedPersons?.Select(x => x.EmailAddressString);
                    var taggedPersonIds = taggedPersons.Select(x => x.Guid);
                    VerifyUserAssignmentsAndInvite(taggedPersonIds, property, true, user.Guid);
                    NotifyTaggedUsers(taggedPersons, note, property, dc);

                    //email / sms to tagged users
                    if (noteRequest.TaggedUsers?.Count > 0)
                    {
                        List<string> sendemails = new List<string>();
                        foreach (var item in noteRequest.TaggedUsers)
                        {
                            if (item.IsSendEmail)
                            {
                                sendemails.Add(item.email);
                            }

                            if (item.IsSendSMS)
                            {
                                string number = item.PhoneNumber;
                                if (!string.IsNullOrEmpty(number))
                                {
                                    _smsService.Value.SendSms("You received new notes", number);
                                }
                            }
                        }
                        SendEmailNotification(note.Content, note.CreatedByName, sendemails, property, emailnoteid, true);
                    }
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
                    ParentID = noteRequest.ParentID,
                    Attachments = noteRequest.Attachments,
                    TaggedUsers = noteRequest.TaggedUsers,
                    IsSendEmail = noteRequest.IsSendEmail,
                    IsSendSMS = noteRequest.IsSendSMS
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
                    throw new HttpResponseException(new HttpResponseMessage() { StatusCode = HttpStatusCode.NotFound, ReasonPhrase = "The note with the specified Guid was not found" });
                }

                //get the user who deleted the note
                var user = dc.People.FirstOrDefault(x => !x.IsDeleted
                                               && (x.SmartBoardID.Equals(userID, StringComparison.InvariantCultureIgnoreCase) || x.EmailAddressString.Equals(email, StringComparison.InvariantCultureIgnoreCase)));
                if (user == null)
                {
                    throw new HttpResponseException(new HttpResponseMessage() { StatusCode = HttpStatusCode.NotFound, ReasonPhrase = "No user found with the specified ID" });
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

        public async Task<IEnumerable<SBNoteData>> NotesCreate(NoteCreateDTO noteRequest, DateTime fromDate, DateTime toDate)
        {
            try
            {
                string userID = string.Join(",", noteRequest.userId);
                string apiKey = string.Join(",", noteRequest.apiKey);

                using (var dc = new DataContext())
                {

                    var NoteList = await dc
                        .Database
                        .SqlQuery<SBNoteData>("exec usp_getnoteDatagroupbyProperty @fromdate, @todate, @apiKey, @userid",
                        new SqlParameter("@fromdate", fromDate),
                        new SqlParameter("@todate", toDate),
                        new SqlParameter("@apiKey", apiKey),
                        new SqlParameter("@userid", userID))
                        .ToListAsync();


                    NoteList.RemoveAll(x => x.LeadID == null || x.apiKey == null || x.DateCreated == null);

                    return NoteList;

                }

            }
            catch (Exception ex)
            {
                throw new HttpResponseException(new HttpResponseMessage() { StatusCode = HttpStatusCode.NotFound, ReasonPhrase = ex.Message });
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
                    throw new HttpResponseException(new HttpResponseMessage() { StatusCode = HttpStatusCode.NotFound, ReasonPhrase = "The Lead with the specified LeadId was not found" });
                }

                if (TerritoryId == null)
                {
                    throw new HttpResponseException(new HttpResponseMessage() { StatusCode = HttpStatusCode.NotFound, ReasonPhrase = "Please select Territory" });
                }

                var territory = dc.Territories.Where(x => x.Guid == TerritoryId).FirstOrDefault();
                if (territory == null)
                {
                    throw new HttpResponseException(new HttpResponseMessage() { StatusCode = HttpStatusCode.NotFound, ReasonPhrase = "The Territory with the specified TerritoryId was not found" });
                }

                //get the user who transfered the Lead Territory
                var user = dc.People.FirstOrDefault(x => !x.IsDeleted
                                               && (x.EmailAddressString.Equals(email)));
                if (user == null)
                {
                    throw new HttpResponseException(new HttpResponseMessage() { StatusCode = HttpStatusCode.NotFound, ReasonPhrase = "No user found with the specified ID" });
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
                return dc.People.Include(p => p.PhoneNumbers).AsNoTracking().Where(x => !x.IsDeleted && emails.Contains(x.EmailAddressString)).ToList();
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
                    .AsNoTracking()
                    .FirstOrDefault(x => x.SmartBoardId == smartboardLeadID || x.Id == igniteID);

                if (property == null)
                {
                    throw new HttpResponseException(new HttpResponseMessage() { StatusCode = HttpStatusCode.NotFound, ReasonPhrase = "No lead found with the specified ID(s)" });
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
                    throw new HttpResponseException(new HttpResponseMessage() { StatusCode = HttpStatusCode.NotFound, ReasonPhrase = "Please send Valid Apikey base on LeadId." });
                }

                return property;
            }

        }

        public async Task<string> getApiKey(long? smartboardLeadID, long? igniteID, string apiKey)
        {
            using (var dc = new DataContext())
            {
                if (!smartboardLeadID.HasValue)
                {
                    return null;
                }
                //first get the property
                var property = await dc.Properties.AsNoTracking().FirstOrDefaultAsync(x => x.SmartBoardId == smartboardLeadID || x.Id == igniteID);

                if (property == null)
                {
                    throw new HttpResponseException(new HttpResponseMessage() { StatusCode = HttpStatusCode.NotFound, ReasonPhrase = "No lead found with the specified ID(s)" });
                }
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

        public async Task<IEnumerable<Territories>> GetTerritoriesList(long smartboardLeadID, string apiKey)
        {
            using (var dc = new DataContext())
            {
                //first get the property
                var property = await dc.Properties.FirstOrDefaultAsync(x => x.SmartBoardId == smartboardLeadID);

                if (property == null)
                {
                    throw new HttpResponseException(new HttpResponseMessage() { StatusCode = HttpStatusCode.NotFound, ReasonPhrase = "No lead found with the specified ID(s)" });
                }

                //-- exec usp_GetTerritoryIdsNameByapiKey 29.973433, -95.243265, '1f82605d3fe666478f3f4f1ee25ae828'
                var TerritoriesList = await dc
             .Database
             .SqlQuery<Territories>("exec usp_GetTerritoryIdsNameByapiKey @latitude, @longitude, @apiKey", new SqlParameter("@latitude", property.Latitude), new SqlParameter("@longitude", property.Longitude), new SqlParameter("@apiKey", apiKey))
             .ToListAsync();

                return TerritoriesList;
            }
        }
        private void SendEmailForNotesComment(string content, string Username, string email, Property property, Guid noteID, bool IsSmartboard = false)
        {
            Task.Factory.StartNew(() =>
            {
                var directNoteLinks = $"<a href='{Constants.APIBaseAddress}/home/redirect?notes?propertyID={property.Guid}&noteID={noteID}'>Click here to open the note directly in IGNITE (Link only works on iOS devices)</a><br/> <a href='{Constants.SmartboardURL}/leads/view/{property.SmartBoardId}?showNote=1&note_id={noteID}'>Click here to open the note directly in SMARTBoard</a>";

                var body = $"Note Sent by: {Username}<br/><br/>New Comment on note you were created. <br/> The note is for {property.Name} at {property.Address1} {property.City}, {property.State}. <br/> Here's the note content: <br/><br/> {content} . <br/><br/><b>Do not Reply</b><br/><br/>{directNoteLinks}";

                Mail.Library.SendEmail(email, string.Empty, $"New Comment on note for {property.Name} at {property.Address1} {property.City}, {property.State}", body, true, null, IsSmartboard);
            });

        }

        private void SendEmailNotification(string content, string Username, IEnumerable<string> emails, Property property, Guid noteID, bool IsSmartboard = false)
        {
            Task.Factory.StartNew(() =>
            {
                emails = emails.Distinct();
                var tag1Regex = new Regex(@"\[email:'(.*?)'\]");
                var tag2Regex = new Regex(@"\[\/email\]");

                content = tag1Regex.Replace(content, "<b>");
                content = tag2Regex.Replace(content, "</b>");

                var directNoteLinks = $"<a href='{Constants.APIBaseAddress}/home/redirect?notes?propertyID={property.Guid}&noteID={noteID}'>Click here to open the note directly in IGNITE (Link only works on iOS devices)</a><br/> <a href='{Constants.SmartboardURL}/leads/view/{property.SmartBoardId}?showNote=1&note_id={noteID}'>Click here to open the note directly in SMARTBoard</a>";

                var body = $"Note Sent by: {Username}<br/><br/>New activity has been recorded on a note you were tagged in. <br/> The note is for {property.Name} at {property.Address1} {property.City}, {property.State}. <br/> Here's the note content: <br/><br/> {content} . <br/><br/><b>Do not Reply</b><br/><br/>{directNoteLinks}";
                var to = string.Join(";", emails);

                Mail.Library.SendEmail(to, string.Empty, $"New note for {property.Name} at {property.Address1} {property.City}, {property.State}", body, true, null, IsSmartboard);
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

        public async Task<string> UpdateSmartboardIdByEmail()
        {
            //send notification 
            var response = await _sbAdapter.Value.GetAllSbUsers();

            if (response?.users?.Count > 0)
            {
                try
                {
                    //update the user's SmartBoard ID
                    using (var dc = new DataContext())
                    {
                        foreach (var item in response?.users)
                        {
                            var currentPerson = await dc.People.FirstOrDefaultAsync(x => x.EmailAddressString == item.email);
                            if (currentPerson != null)
                            {
                                if (item.id != Convert.ToInt32(currentPerson.SmartBoardID))
                                {
                                    currentPerson.SmartBoardID = item.id.ToString();
                                    currentPerson.Updated();
                                    ApiLogEntry apilog = new ApiLogEntry();
                                    apilog.Id = Guid.NewGuid();
                                    apilog.User = SmartPrincipal.UserId.ToString();
                                    apilog.Machine = Environment.MachineName;
                                    apilog.RequestContentType = "Update All Smartboard IDS";
                                    apilog.RequestTimestamp = DateTime.UtcNow;
                                    apilog.RequestUri = "UpdateSmartboardIdByEmail";
                                    apilog.ResponseContentBody = $"IGNITE-SmartBoardID {currentPerson.SmartBoardID} SmartBoardID {item.id}";

                                    dc.ApiLogEntries.Add(apilog);
                                    await dc.SaveChangesAsync();
                                }
                            }
                        }

                        await dc.SaveChangesAsync();

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

