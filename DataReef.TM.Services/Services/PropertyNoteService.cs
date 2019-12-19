﻿using DataReef.Core.Infrastructure.Repository;
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

        public PropertyNoteService(
            ILogger logger,
            Func<IUnitOfWork> unitOfWorkFactory,
            Lazy<IOUSettingService> ouSettingService,
            Lazy<IOUService> ouService,
            Lazy<IAssignmentService> assignmentService,
            Lazy<IUserInvitationService> userInvitationService,
            Lazy<IPersonService> personService,
            Lazy<ISolarSalesTrackerAdapter> sbAdapter) : base(logger, unitOfWorkFactory)
        {
            _ouSettingService = ouSettingService;
            _ouService = ouService;
            _assignmentService = assignmentService;
            _userInvitationService = userInvitationService;
            _personService = personService;
            _sbAdapter = sbAdapter;
        }

        public IEnumerable<PropertyNote> GetNotesByPropertyID(Guid propertyID)
        {
            using (var dc = new DataContext())
            {
                //get property along with the notes
                var notes = dc.PropertyNotes.Where(p => p.PropertyID == propertyID && !p.IsDeleted).ToList();
                return notes ?? new List<PropertyNote>();
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

                var property = dc.Properties.Include(x => x.Territory).FirstOrDefault(x => x.Guid == entity.PropertyID);

                if (property != null)
                {
                    //send notifications to the tagged users
                    var taggedPersons = GetTaggedPersons(entity.Content);
                    if (taggedPersons?.Any() == true)
                    {
                        var emails = taggedPersons?.Select(x => x.EmailAddressString);
                        var taggedPersonIds = taggedPersons.Select(x => x.Guid);
                        VerifyUserAssignmentsAndInvite(taggedPersonIds, property);
                        if (emails?.Any() == true)
                        {
                            SendEmailNotification(entity.Content, emails, property, entity.Guid);
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
                var property = dc.Properties.Include(x => x.Territory).FirstOrDefault(x => x.Guid == entity.PropertyID);

                if (property != null)
                {
                    //send notifications to the tagged users
                    var taggedPersons = GetTaggedPersons(entity.Content);
                    if (taggedPersons?.Any() == true)
                    {
                        var emails = taggedPersons?.Select(x => x.EmailAddressString);
                        var taggedPersonIds = taggedPersons.Select(x => x.Guid);
                        VerifyUserAssignmentsAndInvite(taggedPersonIds, property);
                        if (emails?.Any() == true)
                        {
                            SendEmailNotification(entity.Content, emails, property, entity.Guid);
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
                    var property = properties.FirstOrDefault(p => p.Guid == entity.PropertyID);
                    if (property != null)
                    {
                        //send notifications to the tagged users
                        var taggedPersons = GetTaggedPersons(entity.Content);
                        if (taggedPersons?.Any() == true)
                        {
                            var emails = taggedPersons?.Select(x => x.EmailAddressString);
                            var taggedPersonIds = taggedPersons.Select(x => x.Guid);
                            VerifyUserAssignmentsAndInvite(taggedPersonIds, property);
                            if (emails?.Any() == true)
                            {
                                SendEmailNotification(entity.Content, emails, property, entity.Guid);
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
                var property = dc.Properties.Include(x => x.Territory).FirstOrDefault(x => x.Guid == entity.PropertyID);

                if (property != null)
                {
                    //send notifications to the tagged users
                    var taggedPersons = GetTaggedPersons(entity.Content);
                    if (taggedPersons?.Any() == true)
                    {
                        var emails = taggedPersons?.Select(x => x.EmailAddressString);
                        var taggedPersonIds = taggedPersons.Select(x => x.Guid);
                        VerifyUserAssignmentsAndInvite(taggedPersonIds, property);
                        if (emails?.Any() == true)
                        {
                            SendEmailNotification(entity.Content, emails, property, entity.Guid);
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
                    var property = properties.FirstOrDefault(p => p.Guid == entity.PropertyID);
                    if (property != null)
                    {
                        //send notifications to the tagged users
                        var taggedPersons = GetTaggedPersons(entity.Content);
                        if (taggedPersons?.Any() == true)
                        {
                            var emails = taggedPersons?.Select(x => x.EmailAddressString);
                            var taggedPersonIds = taggedPersons.Select(x => x.Guid);
                            VerifyUserAssignmentsAndInvite(taggedPersonIds, property);
                            if (emails?.Any() == true)
                            {
                                SendEmailNotification(entity.Content, emails, property, entity.Guid);
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
                    UserID = users?.FirstOrDefault(u => u.Guid == x.PersonID)?.SmartBoardID

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

                //send notifications to the tagged users
                var taggedPersons = GetTaggedPersons(note.Content);
                if (taggedPersons?.Any() == true)
                {
                    var emails = taggedPersons?.Select(x => x.EmailAddressString);
                    var taggedPersonIds = taggedPersons.Select(x => x.Guid);
                    VerifyUserAssignmentsAndInvite(taggedPersonIds, property);
                    if (emails?.Any() == true)
                    {
                        SendEmailNotification(note.Content, emails, property, note.Guid);
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

                var taggedPersons = GetTaggedPersons(note.Content);
                if (taggedPersons?.Any() == true)
                {
                    var emails = taggedPersons?.Select(x => x.EmailAddressString);
                    var taggedPersonIds = taggedPersons.Select(x => x.Guid);
                    VerifyUserAssignmentsAndInvite(taggedPersonIds, property);
                    if (emails?.Any() == true)
                    {
                        SendEmailNotification(note.Content, emails, property, note.Guid);
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
                    UserID = smartboardUserID
                };
            }
        }

        public void DeleteNoteFromSmartboard(Guid noteID, string userID, string apiKey, string email)
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
                    throw new Exception("No lead found with the specified ID");
                }

                return property;
            }

        }

        private void SendEmailNotification(string content, IEnumerable<string> emails, Property property, Guid noteID)
        {
            Task.Factory.StartNew(() =>
            {
                emails = emails.Distinct();
                var tag1Regex = new Regex(@"\[email:'(.*?)'\]");
                var tag2Regex = new Regex(@"\[\/email\]");

                content = tag1Regex.Replace(content, "<b>");
                content = tag2Regex.Replace(content, "</b>");

                var directNoteLinks = $"<a href='{Constants.CustomURL}notes?propertyID={property.Guid}&noteID={noteID}'>Click here to open the note directly in IGNITE (Link only works on iOS devices)</a><br/> <a href='{Constants.SmartboardURL}/leads/view/{property.SmartBoardId}?showNote=1&note_id={noteID}'>Click here to open the note directly in SmartBoard</a>";
                var body = $"New activity has been recorded on a note you were tagged in. <br/> The note is for {property.Name} at {property.Address1} {property.City}, {property.State}. <br/> Here's the note content: <br/><br/> {content} <br/><br/>{directNoteLinks}";
                var to = string.Join(";", emails);

                Mail.Library.SendEmail(to, string.Empty, $"New note for {property.Name} at {property.Address1} {property.City}, {property.State}", body, true);
            });
        }

        private void VerifyUserAssignmentsAndInvite(IEnumerable<Guid> userIds, Property property)
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
                                    FromPersonID = SmartPrincipal.UserId,
                                    EmailAddress = person.EmailAddressString,
                                    FirstName = person.FirstName,
                                    LastName = person.LastName,
                                    OUID = property.Territory.OUID,
                                    RoleID = OURole.MemberRoleID,
                                    CreatedByID = SmartPrincipal.UserId,
                                };
                                _userInvitationService.Value.Insert(invite);
                            }

                            //assign user to the territory
                            _assignmentService.Value.Insert(new Assignment
                            {
                                CreatedByID = SmartPrincipal.UserId,
                                PersonID = person.Guid,
                                TerritoryID = property.TerritoryID,
                                Status = AssignmentStatus.Open
                            });
                        }
                    }
                }
            }


        }
    }
}
