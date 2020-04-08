using DataReef.Core;
using DataReef.Core.Classes;
using DataReef.Core.Infrastructure.Repository;
using DataReef.Core.Logging;
using DataReef.TM.Classes;
using DataReef.TM.Contracts.Services;
using DataReef.TM.DataAccess.Database;
using DataReef.TM.Models;
using DataReef.TM.Models.DTOs.Integrations;
using DataReef.TM.Models.DTOs.Persons;
using DataReef.TM.Services.Services.FinanceAdapters.SolarSalesTracker;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;

namespace DataReef.TM.Services
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(AddressFilterMode = AddressFilterMode.Any)]
    public class UserInvitationService : DataService<UserInvitation>, IUserInvitationService
    {
        private readonly IOUService _ouService;

        public UserInvitationService(ILogger logger, IOUService ouService, Func<IUnitOfWork> unitOfWorkFactory)
            : base(logger, unitOfWorkFactory)
        {
            _ouService = ouService;
        }

        public override UserInvitation Get(Guid uniqueId, string include = "", string exclude = "", string fields = "", bool deletedItems = false)
        {
            var userInvitation = base.Get(uniqueId, include, exclude, fields, deletedItems);
            if (include.Contains("OU") && userInvitation.OU != null)
            {
                userInvitation.OU = _ouService.OUBuilder(userInvitation.OU);
            }
            return userInvitation;
        }

        public override System.Collections.Generic.ICollection<UserInvitation> List(bool deletedItems = false, int pageNumber = 1, int itemsPerPage = 20, string filter = "", string include = "", string exclude = "", string fields = "")
        {
            var userInvitations = base.List(deletedItems, pageNumber, itemsPerPage, filter, include, exclude, fields);
            if (include.Contains("OU"))
            {
                foreach (var userInvitation in userInvitations)
                {
                    if (userInvitation.OU != null)
                    {
                        userInvitation.OU = _ouService.OUBuilder(userInvitation.OU);
                    }
                }
            }
            return userInvitations;
        }

        public UserInvitation SilentInsertFromSmartboard(CreateUserDTO user, string apiKey)
        {
            using (var dc = new DataContext())
            {
                //get the OU based on the apiKey
                var ouSetting = dc
                    .OUSettings
                    .Where(x => x.Name == SolarTrackerResources.SelectedSettingName)
                    .ToList()
                    .FirstOrDefault(x =>
                    {
                        var selectedIntegrations = x.GetValue<ICollection<SelectedIntegrationOption>>();
                        return selectedIntegrations.Any(s => s?.Data?.SMARTBoard?.ApiKey == apiKey);

                    });

                if (ouSetting == null)
                {
                    return null;
                }

                var userInvitation = new UserInvitation
                {
                    EmailAddress = user.EmailAddress,
                    ExpirationDate = DateTime.UtcNow.AddDays(30),
                    DateCreated = DateTime.UtcNow,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    IsDeleted = false,
                    InvitationCode = Guid.NewGuid().ToString(),
                    Status = InvitationStatus.Pending,
                    Name = "New UserInvitation",
                    RoleID = user.RoleID,
                    OUID = ouSetting.OUID,
                    FromPersonID = Guid.Parse(ConfigurationManager.AppSettings["AnonymousPrincipal_UserID"])

                };

                return InsertEntity(userInvitation, dc, false);
            }
        }

        public override UserInvitation Insert(UserInvitation entity, DataContext dataContext)
        {
            return InsertEntity(entity, dataContext, true);
        }

        private UserInvitation InsertEntity(UserInvitation entity, DataContext dataContext, bool sendEmail = false)
        {
            bool userWasDeleted = false;
            string fromName = string.Empty;
            string ouName = string.Empty;

            UserInvitation userInvitation;
            var org = dataContext.OUs.FirstOrDefault(ou => ou.Guid == entity.OUID);

            if (org == null)
            {
                throw new ApplicationException("Invalid OUID.  OU does not exist");
            }

            ouName = org.Name;

            var sender = dataContext.People.FirstOrDefault(p => p.Guid == entity.FromPersonID);

            if (sender == null)
            {
                throw new ArgumentException("Invalid FromPersonID. Person does not exist");
            }

            fromName = string.Format("{0} {1}", sender.FirstName, sender.LastName);

            entity.EmailAddress = entity.EmailAddress.ToLower();

            Guid? toPersonId = entity.ToPersonID;
            if (!toPersonId.HasValue)
            {
                try
                {
                    var existingPersonId = dataContext.People.Where(p => p.EmailAddressString.Contains(entity.EmailAddress)).Select(p => p.Guid).SingleOrDefault();
                    if (existingPersonId != Guid.Empty)
                    {
                        toPersonId = existingPersonId;
                        entity.ToPersonID = toPersonId;
                    }
                }
                catch { }
            }

            if (entity.FromPersonID == entity.ToPersonID) throw new ApplicationException("You are not allowed to invite yourself to this organization!");

            if (toPersonId.HasValue)
            {
                var person = dataContext.People.SingleOrDefault(p => p.Guid == toPersonId.Value);
                if (person != null && person.IsDeleted)
                {
                    userWasDeleted = true;
                    person.IsDeleted = false;
                }

                var user = dataContext.Users.SingleOrDefault(p => p.Guid == toPersonId.Value);
                if (user != null && user.IsDeleted)
                {
                    userWasDeleted = true;
                    user.IsDeleted = false;
                }

                var credentials = dataContext.Credentials.Where(c => c.UserID == toPersonId.Value).ToList();
                if (credentials != null && credentials.Any())
                {
                    credentials.ForEach(c => c.IsDeleted = false);
                }

                dataContext.SaveChanges();
            }


            userInvitation = dataContext.UserInvitations.FirstOrDefault(ui =>
                                                (ui.EmailAddress == entity.EmailAddress || (ui.ToPersonID != null && ui.ToPersonID == entity.ToPersonID)) &&
                                                ui.OUID == entity.OUID &&
                                                ui.Status == InvitationStatus.Pending &&
                                                !ui.DateAccepted.HasValue);

            if (userInvitation != null)
            {

                if (toPersonId.HasValue && userInvitation.ToPersonID != toPersonId)
                {
                    userInvitation.ToPersonID = toPersonId;
                }

                // reset the expiration date
                userInvitation.SetExpirationDate();

                if (userInvitation.IsDeleted)
                {
                    userInvitation.IsDeleted = false;
                }

                dataContext.SaveChanges();

                userInvitation.SaveResult = new SaveResult { Success = true };
            }
            else
            {
                userInvitation = base.Insert(entity, dataContext);
            }

            // Send or resend the invitation
            if (sendEmail)
            {
                SendUserInvitationEmail(userInvitation, fromName, ouName, userWasDeleted);
            }

            return userInvitation;
        }

        public override UserInvitation Insert(UserInvitation entity)
        {
            using (DataContext dc = new DataContext())
            {
                return Insert(entity, dc);
            }
        }

        public override ICollection<UserInvitation> InsertMany(ICollection<UserInvitation> entities)
        {
            var userInvitations = new List<UserInvitation>();
            foreach (var entity in entities)
            {
                userInvitations.Add(Insert(entity));
            }
            return userInvitations;
        }

        public override UserInvitation Update(UserInvitation entity)
        {
            string toName = string.Empty;
            string fromName = string.Empty;
            string ouName = string.Empty;
            string roleName = string.Empty;

            entity.SetExpirationDate();
            var result = base.Update(entity);

            using (DataContext dc = new DataContext())
            {
                var org = dc.OUs.FirstOrDefault(ou => ou.Guid == entity.OUID);

                if (org == null)
                {
                    throw new ApplicationException("Invalid OUID.  OU does not exist");
                }

                ouName = org.Name;

                var sender = dc.People.FirstOrDefault(p => p.Guid == entity.FromPersonID);

                if (sender == null)
                {
                    throw new ArgumentException("Invalid FromPersonID. Person does not exist");
                }

                fromName = string.Format("{0} {1}", sender.FirstName, sender.LastName);
            }

            if (result.Status == InvitationStatus.Accepted)
            {
                using (DataContext dc = new DataContext())
                {
                    Guid personId = Guid.Empty;
                    var emailAddress = dc
                                        .People
                                        .Where(p => p.Guid == result.FromPersonID)
                                        .Select(p => p.EmailAddressString)
                                        .FirstOrDefault();
                    var role = dc
                                .OURoles
                                .FirstOrDefault(r => r.Guid == result.RoleID);
                    roleName = role
                                .Name;

                    if (result.ToPersonID.HasValue)
                    {
                        personId = result.ToPersonID.Value;
                    }
                    else
                    {
                        // if the Invitation does not have the ToPersonId, try to get it from People based on the email
                        var person = dc.People.FirstOrDefault(p => p.EmailAddressString.Contains(result.EmailAddress) && !p.IsDeleted);
                        if (person != null) personId = person.Guid;
                    }

                    if (personId != Guid.Empty)
                    {
                        //check to see if the user is already part of the OU
                        OUAssociation ouAssociation = dc.OUAssociations.FirstOrDefault(oua => oua.PersonID == personId && oua.OUID == result.OUID);
                        var recipient = dc.People.FirstOrDefault(p => p.Guid == personId);
                        if (recipient != null)
                        {
                            toName = string.Format("{0} {1}", recipient.FirstName, recipient.LastName);
                        }
                        else
                        {
                            toName = string.Format("{0} {1}", result.FirstName, result.LastName);
                        }

                        if (ouAssociation == null)
                        {
                            //add the OU association and the Role to that Association
                            ouAssociation = new OUAssociation()
                            {
                                OUID = result.OUID,
                                PersonID = personId,
                                OURoleID = result.RoleID,
                                RoleType = role.RoleType,
                            };
                            dc.OUAssociations.Add(ouAssociation);
                            dc.SaveChanges();
                        }
                        else
                        {
                            bool needToSave = false;

                            if (ouAssociation.IsDeleted)
                            {
                                ouAssociation.IsDeleted = false;
                                needToSave = true;
                            }

                            if (ouAssociation.OURoleID != result.RoleID)
                            {
                                ouAssociation.OURoleID = result.RoleID;
                                ouAssociation.RoleType = role.RoleType;
                                needToSave = true;
                            }

                            if (needToSave)
                            {
                                dc.SaveChanges();
                            }

                        }
                    }
                    SendUserInvitationAcceptedEmail(result, emailAddress, toName, ouName, roleName);
                }
            }
            else // pending or expired, resend invitation email
            {
                SendUserInvitationEmail(result, fromName, ouName);
            }
            return result;
        }

        private void SendUserInvitationEmail(UserInvitation ui, string fromName, string ouName, bool userWasDeleted = false)
        {
            var template = new UserInvitationTemplate
            {
                ToPersonName = string.Format("{0} {1}", ui.FirstName, ui.LastName),
                ToPersonEmail = ui.EmailAddress,
                DownloadURL = ConfigurationManager.AppSettings["LegionDownloadURL"],
                InviterName = fromName,
                FromPersonName = Constants.FromEmailName,
                ToPersonId = ui.ToPersonID,
                OUName = ouName,
                Guid = ui.Guid,
                UserWasDeleted = userWasDeleted
            };

            Mail.Library.SendUserInvitationEmail(template);
        }

        private void SendUserInvitationAcceptedEmail(UserInvitation ui, string recipientEmail, string toName, string ouName, string roleName)
        {
            var template = new UserInvitationAcceptedTemplate
            {
                ToPersonName = toName,
                RecipientEmailAddress = recipientEmail,
                ToPersonEmail = ui.EmailAddress,
                FromPersonName = Constants.FromEmailName,
                OUName = ouName,
                RoleName = roleName,
                Guid = ui.Guid
            };

            Mail.Library.SendUserInvitationAcceptedEmail(template);
        }
    }
}
