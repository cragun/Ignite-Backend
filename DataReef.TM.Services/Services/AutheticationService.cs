using DataReef.Core;
using DataReef.Core.Classes;
using DataReef.Core.Infrastructure.Authorization;
using DataReef.Core.Logging;
using DataReef.Integrations.MailChimp;
using DataReef.Integrations.Microsoft;
using DataReef.Integrations.Microsoft.PowerBI.Models;
using DataReef.TM.Classes;
using DataReef.TM.Contracts.Auth;
using DataReef.TM.Contracts.Faults;
using DataReef.TM.Contracts.Services;
using DataReef.TM.DataAccess.Database;
using DataReef.TM.Models;
using DataReef.TM.Models.Accounting;
using DataReef.TM.Models.DataViews;
using DataReef.TM.Models.DTOs.Blobs;
using DataReef.TM.Models.DTOs.Integrations;
using DataReef.TM.Models.DTOs.Persons;
using DataReef.TM.Models.Enums;
using DataReef.TM.Services;
using DataReef.TM.Services.Services.FinanceAdapters.SolarSalesTracker;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Threading.Tasks;

namespace DataReef.Application.Services
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(AddressFilterMode = AddressFilterMode.Any)]
    public class AuthenticationService : IAuthenticationService
    {
        private readonly string InvitationAcceptRecipient = ConfigurationManager.AppSettings["DataReef.Invitations.Accept.NotificationEmail"];
        private const int TOKEN_EXPIRATION_DAYS = 15;
        public const int MaxNumberOfDevicesPerUser = 2;
        private const string CUSTOM_TOKEN_VALIDITY = "Auth Token Validity";
        private ILogger logger = null;
        private readonly Lazy<IMailChimpAdapter> _mailChimpAdapter;
        private readonly Lazy<IBlobService> _blobService;
        private readonly Lazy<IDataService<PasswordReset>> _resetService;
        private readonly Lazy<IDataService<Credential>> _credentialService;
        private readonly Lazy<IPowerBIBridge> _powerBIBridge;
        private readonly Lazy<ISolarSalesTrackerAdapter> _sbAdapter;
        private readonly IAppSettingService _appSettingService = null;

        public AuthenticationService(ILogger logger,
            Lazy<IMailChimpAdapter> mailChimpAdapter,
            Lazy<IBlobService> blobService,
            Lazy<IDataService<PasswordReset>> resetService,
            Lazy<IDataService<Credential>> credentialService,
            Lazy<IPowerBIBridge> powerBIBridge,
            Lazy<ISolarSalesTrackerAdapter> sbAdapter,
            IAppSettingService appSettingService)
        {
            this.logger = logger;
            _mailChimpAdapter = mailChimpAdapter;
            _blobService = blobService;
            _resetService = resetService;
            _credentialService = credentialService;
            _powerBIBridge = powerBIBridge;
            _sbAdapter = sbAdapter;
            _appSettingService = appSettingService;
        }

        public AuthenticationToken ChangePassword(string userName, string oldPassword, string newPassword, string fcm_token)
        {
            try
            {
                if (oldPassword == newPassword)
                {
                    throw new ApplicationException("Passwords cannot be the same");
                }

                else if (string.IsNullOrWhiteSpace(newPassword))
                {
                    throw new ApplicationException("User must provide a valid password");
                }
                else if (newPassword.Length <= 6)
                {
                    throw new ApplicationException("User must provide a valid password of 6 characters or more");
                }


                Credential c = _credentialService.Value.List(false, 1, 1, string.Format("UserName={0}", userName)).FirstOrDefault();

                if (c == null)
                {
                    throw new ApplicationException("User Name was no longer found in database. This is a rare exception");
                }

                string hash = DataReef.Auth.Helpers.CryptographyHelper.ComputePasswordHash(oldPassword, c.Salt);
                if (hash == c.PasswordHashed)
                {
                    c.PasswordRaw = newPassword;
                    c.RequiresPasswordChange = false;
                    c.PerformHash();
                    _credentialService.Value.Update(c);
                }
                else
                {
                    throw new ApplicationException("Invalid Password (Old Password) ");
                }

                AuthenticationToken token = this.Authenticate(userName, newPassword , fcm_token);
                return token;

            }
            catch (Exception)
            {
                throw;
            }
        }

        public AuthenticationToken CompletePasswordReset(Guid resetGuid, string newPassword , string fcm_token)
        {
            try
            {
                PasswordReset reset = _resetService.Value.Get(resetGuid);

                if (reset == null)
                {
                    throw new ApplicationException("Invalid Reset Guid");
                }

                else if (string.IsNullOrWhiteSpace(newPassword))
                {
                    throw new ApplicationException("User must provide a valid password");
                }
                else if (newPassword.Length < 6)
                {
                    throw new ApplicationException("User must provide a valid password of 6 characters or more");
                }

                else if (reset.DateReset.HasValue)
                {
                    throw new ApplicationException("Password Reset Has Already Been Used");
                }
                else if (System.DateTime.UtcNow.Ticks > reset.ExpirationDate.Value.Ticks)
                {
                    throw new ApplicationException("Password Reset Has Expired");
                }

                Credential c = _credentialService.Value.List(false, 1, 1, string.Format("UserName={0}", reset.EmailAddress)).FirstOrDefault();

                if (c == null)
                {
                    throw new ApplicationException("User Name was no longer found in database. This is a rare exception");
                }

                c.PasswordRaw = newPassword;
                c.PerformHash();
                c.RequiresPasswordChange = false;
                _credentialService.Value.Update(c);

                reset.DateReset = System.DateTime.UtcNow;
                reset.PasswordWasReset = true;
                _resetService.Value.Update(reset);

                AuthenticationToken token = this.Authenticate(reset.EmailAddress, newPassword, fcm_token);
                return token;

            }
            catch (Exception)
            {
                throw;
            }

        }

        public SaveResult InitiatePasswordReset(PasswordReset resetObject)
        {
            try
            {

                Credential c = _credentialService.Value.List(false, 1, 1, string.Format("UserName={0}", resetObject.EmailAddress), "User.Person").FirstOrDefault();
                if (c == null)
                {
                    return new SaveResult { Success = false, ExceptionMessage = "Unknown Email Address" };
                }
                else if (c.IsDeleted || c.User.Person == null || c.User.Person.IsDeleted)
                {
                    return new SaveResult { Success = false, ExceptionMessage = "Account is suspended!" };
                }
                else if (c.User.IsDisabled == true)
                {
                    return new SaveResult { Success = false, ExceptionMessage = "User Account Is Disabled" };
                }

                string name = c.User.Person.FirstName;
                resetObject.PersonID = c.User.PersonID;
                resetObject.UserName = c.UserName;

                PasswordReset obj = _resetService.Value.Insert(resetObject);

                if (obj.SaveResult.Success)
                {
                    var template = new ResetPasswordTemplate
                    {
                        ToPersonName = name,
                        ToPersonEmail = resetObject.EmailAddress,
                        FromPersonName = Constants.FromEmailName,
                        Guid = resetObject.Guid
                    };
                    DataReef.TM.Mail.Library.SendPassworReset(template);

                }
                else
                {
                    return new SaveResult { Success = false, ExceptionMessage = "Unable to Save Password Reset :" + obj.SaveResult.ExceptionMessage };
                }

                return obj.SaveResult;
            }
            catch (Exception ex)
            {
                return new SaveResult { Success = false, ExceptionMessage = ex.Message };
            }
        }

        public AuthenticationToken Authenticate(string userName, string password , string fcm_token)
        {
            using (DataContext dc = new DataContext())
            {

                var credentials = dc
                                .Credentials
                                .Include(cred => cred.User.Person.PersonSettings)
                                .Include(cred => cred.User.UserDevices)
                                .Where(cc => cc.UserName == userName)
                                .ToList();

                // there are cases when there are multiple credentials, some of them deleted
                // we try to get the 1st one that's not deleted
                Credential c = credentials
                            .FirstOrDefault(cred => !cred.IsDeleted
                                                 && !cred.User.IsDeleted
                                                 && !cred.User.Person.IsDeleted);

                // if there are not credentials, or all are deleted
                // we'll get the first one ... or the default (null)
                if (c == null)
                {
                    c = credentials.FirstOrDefault();
                }

                if (c != null)
                {
                    if (c.IsDeleted ||
                        c.User.IsDeleted
                        || (c.User.Person != null && c.User.Person.IsDeleted))
                    {
                        throw new ArgumentException("Account is suspended!");
                    }

                    string hash = DataReef.Auth.Helpers.CryptographyHelper.ComputePasswordHash(password, c.Salt);
                    if (hash == c.PasswordHashed)
                    {
                        if (c.User.IsDisabled)
                        {
                            throw new ArgumentException("User is Disabled");
                        }
                        else if (c.RequiresPasswordChange)
                        {
                            throw new ArgumentException("Credential Requires Password Change");
                        }
                        else
                        {
                            if (c.User != null && c.User.UserDevices != null)
                            {
                                var userDevices = c
                                                    .User
                                                    .UserDevices
                                                    .ToList();

                                var currentDevice = userDevices.FirstOrDefault(d => d.DeviceID == SmartPrincipal.DeviceId);

                                DeviceService.HandleDevice(dc, c.User.NumberOfDevicesAllowed, userDevices, currentDevice, SmartPrincipal.DeviceId, c.UserID);
                            }

                            //TODO: refactor to throw an error if multi account access and missing an accountid from the header
                            //for now just grab the first one
                            AccountAssociation aa = dc.AccountAssociations.FirstOrDefault(aas => aas.PersonID == c.User.PersonID);

                            var setting = c
                                            .User?
                                            .Person?
                                            .PersonSettings?
                                            .FirstOrDefault(s => !s.IsDeleted && s.Name == CUSTOM_TOKEN_VALIDITY);

                            var expirationDays = TOKEN_EXPIRATION_DAYS;
                            if (setting != null)
                            {
                                int.TryParse(setting.Value, out expirationDays);
                            }

                            using (DataContext db = new DataContext())
                            {
                                var per = db.People.Where(p => p.Guid == c.User.PersonID).FirstOrDefault();
                                if (per != null)
                                {
                                    per.LastLoginDate = DateTime.UtcNow;
                                    if (!String.IsNullOrEmpty(fcm_token))
                                    {
                                        per.fcm_token = fcm_token;
                                    }
                                    db.SaveChanges();
                                }
                            }

                            var dayvalidation = dc.Authentications.Where(a => a.UserID == c.UserID).ToList();

                            int logindays = _appSettingService.GetLoginDays();

                            DateTime oldDate = System.DateTime.UtcNow.AddDays(-(logindays));

                            var lastLoginCount = dayvalidation.Count(id => id.DateAuthenticated.Date >= oldDate.Date);

                            if (dayvalidation.Count > 0 && lastLoginCount == 0)
                            {

                                var person = dc
                                     .People
                                     .SingleOrDefault(p => p.Guid == c.UserID
                                                  && p.IsDeleted == false);

                                if (person != null && !person.IsDeleted)
                                {
                                    person.IsDeleted = true;
                                }
                                var user = dc
                                     .Users
                                     .SingleOrDefault(u => u.PersonID == c.UserID
                                                && u.IsDeleted == false);

                                if (user != null && !user.IsDeleted)
                                {
                                    user.IsDeleted = true;
                                }
                                var credential = dc
                                    .Credentials
                                    .SingleOrDefault(u => u.PersonID == c.UserID
                                               && u.IsDeleted == false);

                                if (credential != null && !credential.IsDeleted)
                                {
                                    credential.IsDeleted = true;
                                }
                                dc.SaveChanges();

                                throw new ArgumentException("Account is suspended!");
                            }

                            AuthenticationToken token = new AuthenticationToken();
                            token.Audience = "tm";
                            token.ClientSecret = "asdfjkl;qweruipo";
                            token.Expiration = System.DateTime.UtcNow.AddDays(expirationDays).ToUnixTime();
                            token.UserID = c.UserID;
                            if (aa != null) token.AccountID = aa.AccountID;

                            Task.Run(() =>
                            {
                                using (DataContext dataContext = new DataContext())
                                {
                                    dataContext.Authentications.Add(new Authentication
                                    {
                                        Guid = Guid.NewGuid(),
                                        UserID = c.UserID,
                                        DeviceID = SmartPrincipal.DeviceId,
                                        DateAuthenticated = DateTime.UtcNow
                                    });
                                    dataContext.SaveChanges();
                                }
                            });


                            var pbi = new PBI_ActiveUser
                            {
                                UserID = c.UserID,
                                DeviceID = SmartPrincipal.DeviceId
                            };

                            _powerBIBridge.Value.PushDataAsync(pbi);


                            return token;
                        }
                    }
                    else
                    {
                        throw new ArgumentException("Invalid User Name or Password");
                    }
                }
                else
                {
                    throw new ArgumentException("Invalid User Name or Password");
                }
            }

        }

        public bool updateUser(bool value, Guid userId)
        {
            using (DataContext dc = new DataContext())
            {
                var person = dc
                     .People
                     .SingleOrDefault(p => p.Guid == userId);
                var user = dc
                     .Users
                     .SingleOrDefault(u => u.PersonID == userId);
                var credential = dc
                     .Credentials
                     .SingleOrDefault(u => u.PersonID == userId);
                if (person != null && user != null && credential != null)
                {
                    if (value)
                    {
                        person.IsDeleted = false;
                        user.IsDeleted = false;
                        credential.IsDeleted = false;
                        dc.SaveChanges();
                        return true;
                    }
                    else
                    {
                        person.IsDeleted = true;
                        user.IsDeleted = true;
                        credential.IsDeleted = true;
                        dc.SaveChanges();
                        return true;
                    }
                }
            }
            return false;

        }

        public AuthenticationToken AuthenticateUserBySuperAdmin(Guid personid)
        {
            using (DataContext dc = new DataContext())
            {
                var isSuperAdmin = dc.OUAssociations.Include(oa => oa.OURole).Where(oa => !oa.IsDeleted && oa.PersonID == SmartPrincipal.UserId && oa.OURole.RoleType == OURoleType.SuperAdmin).ToList();

                if (isSuperAdmin.Count == 0)
                {
                    throw new ArgumentException("Super-Admin can access only.");
                }

                var credentials = dc
                                .Credentials
                                .Include(cred => cred.User.Person.PersonSettings)
                                .Include(cred => cred.User.UserDevices)
                                .Where(cc => cc.PersonID == personid)
                                .ToList();

                // there are cases when there are multiple credentials, some of them deleted
                // we try to get the 1st one that's not deleted
                Credential c = credentials
                            .FirstOrDefault(cred => !cred.IsDeleted
                                                 && !cred.User.IsDeleted
                                                 && !cred.User.Person.IsDeleted);

                // if there are not credentials, or all are deleted
                // we'll get the first one ... or the default (null)
                if (c == null)
                {
                    c = credentials.FirstOrDefault();
                }

                if (c != null)
                {
                    if (c.IsDeleted ||
                        c.User.IsDeleted
                        || (c.User.Person != null && c.User.Person.IsDeleted))
                    {
                        throw new ArgumentException("Account is suspended!");
                    }

                    if (c.User.IsDisabled)
                    {
                        throw new ArgumentException("User is Disabled");
                    }
                    else if (c.RequiresPasswordChange)
                    {
                        throw new ArgumentException("Credential Requires Password Change");
                    }
                    else
                    {
                        if (c.User != null && c.User.UserDevices != null)
                        {
                            var userDevices = c
                                                .User
                                                .UserDevices
                                                .ToList();

                            var currentDevice = userDevices.FirstOrDefault(d => d.DeviceID == SmartPrincipal.DeviceId);

                            DeviceService.HandleDevice(dc, c.User.NumberOfDevicesAllowed, userDevices, currentDevice, SmartPrincipal.DeviceId, c.UserID);
                        }

                        //TODO: refactor to throw an error if multi account access and missing an accountid from the header
                        //for now just grab the first one
                        AccountAssociation aa = dc.AccountAssociations.FirstOrDefault(aas => aas.PersonID == c.User.PersonID);

                        var setting = c
                                        .User?
                                        .Person?
                                        .PersonSettings?
                                        .FirstOrDefault(s => !s.IsDeleted && s.Name == CUSTOM_TOKEN_VALIDITY);

                        var expirationDays = TOKEN_EXPIRATION_DAYS;
                        if (setting != null)
                        {
                            int.TryParse(setting.Value, out expirationDays);
                        }

                        AuthenticationToken token = new AuthenticationToken();
                        token.Audience = "tm";
                        token.ClientSecret = "asdfjkl;qweruipo";
                        token.Expiration = System.DateTime.UtcNow.AddDays(expirationDays).ToUnixTime();
                        token.UserID = c.UserID;
                        if (aa != null) token.AccountID = aa.AccountID;
                        return token;
                    }
                }
                else
                {
                    throw new ArgumentException("Invalid User Name or Password");
                }
            }

        }

        public bool IsUserInvitationValid(Guid invitationGuid)
        {
            using (DataContext dc = new DataContext())
            {
                //DH: Spoke w/ Kyle and he said I should removed ExpirationDate from validation
                //return dc.UserInvitations.Where(ur => ur.Guid == invitationGuid && ur.ExpirationDate > System.DateTime.UtcNow && ur.Status == InvitationStatus.Pending).Count() > 0;
                return dc.UserInvitations.Any(ur => ur.Guid == invitationGuid && ur.Status == InvitationStatus.Pending);
            }
        }

        public UserInvitation GetPendingUserInvitation(Guid invitationGuid)
        {
            using (DataContext dc = new DataContext())
            {
                return dc.UserInvitations.FirstOrDefault(ur => ur.Guid == invitationGuid && ur.Status == InvitationStatus.Pending);
            }
        }

        public bool CheckUserExist(string email)
        {
            using (DataContext dc = new DataContext())
            {
                var people = dc.Credentials
                                    .Include(cred => cred.User)
                                    .Include(cred => cred.User.Person)
                                    .FirstOrDefault(cc => cc.UserName == email);
                if (people == null)
                {
                    return false;
                }
                return true;
            }
        }

        [FaultContract(typeof(PreconditionFailedFault))]
        public AuthenticationToken CreateUser(NewUser newUser, byte[] photo = null, string phoneNumber = null)
        {
            using (DataContext dc = new DataContext())
            {
                //first check to make sure that the Registration Guid is valid
                // refactored to only do one DB call for userInvitation, and reuse it later on                
                var userInvitation = dc
                                    .UserInvitations
                                    .Include(ui => ui.OU)
                                    .FirstOrDefault(uui => uui.Guid == newUser.InvitationGuid);

                if (userInvitation == null)
                {
                    string reason = "Invitation doesn't exist!";
                    throw new FaultException<PreconditionFailedFault>(new PreconditionFailedFault(100, reason), reason);
                }

                if (userInvitation.Status != InvitationStatus.Pending)
                {
                    string reason = "Invitation already used!";
                    throw new FaultException<PreconditionFailedFault>(new PreconditionFailedFault(100, reason), reason);
                }

                if (userInvitation.ExpirationDate < DateTime.UtcNow)
                {
                    string reason = "Invitation expired!";
                    throw new FaultException<PreconditionFailedFault>(new PreconditionFailedFault(100, reason), reason);
                }

                //lets first get a handle on the User. see if he/she exists in this account
                userInvitation.Status = InvitationStatus.Accepted;

                // trying to update the userInvitation before continuing
                // to see if this fixes the burst of 4 users getting inserted in a few seconds
                dc.SaveChanges();

                //make sure the OU is valid. We will also need the OUs account ID later on
                var organizationalUnit = dc.OUs.FirstOrDefault(oou => oou.Guid == userInvitation.OUID && !oou.IsDeleted);
                if (organizationalUnit == null)
                {
                    string reason = "Invalid Organization Unit";
                    throw new FaultException<PreconditionFailedFault>(new PreconditionFailedFault(100, reason), reason);
                }

                string emailAddress = userInvitation.EmailAddress;

                //see if a user exists for this emaiAddress
                var credential = dc.Credentials
                                    .Include(cred => cred.User)
                                    .Include(cred => cred.User.Person)
                                    .FirstOrDefault(cc => cc.UserName == emailAddress);

                Person person = null;
                User user = null;
                Guid accountID = System.Guid.Empty;

                if (credential == null)
                {
                    person = new Person
                    {
                        Guid = Guid.NewGuid(),
                        FirstName = newUser.FirstName,
                        LastName = newUser.LastName,
                        EmailAddressString = userInvitation.EmailAddress,
                        Name = string.Format("{0} {1}", newUser.FirstName, newUser.LastName),
                        StartDate = System.DateTime.UtcNow
                };
                    if (!string.IsNullOrEmpty(phoneNumber))
                    {
                        person.PhoneNumbers = new List<PhoneNumber> { new PhoneNumber
                        {
                            PersonID = person.Guid,
                            Number = phoneNumber,
                            PhoneType = PhoneType.Mobile
                        } };
                    }
                    dc.People.Add(person);

                    user = new User
                    {
                        Guid = person.Guid,
                        PersonID = person.Guid,
                        Person = person,
                        NumberOfDevicesAllowed = MaxNumberOfDevicesPerUser
                    };
                    dc.Users.Add(user);

                    var tokenLedger = new TokenLedger
                    {
                        Name = person.Name,
                        UserID = person.Guid,
                        PersonID = person.Guid,
                        IsPrimary = true
                    };
                    dc.TokenLedgers.Add(tokenLedger);

                    credential = new Credential
                    {
                        UserName = userInvitation.EmailAddress,
                        PasswordRaw = newUser.Password,
                        UserID = person.Guid,
                        PersonID = person.Guid,
                    };
                    credential.PerformHash();
                    dc.Credentials.Add(credential);

                }
                else
                {
                    person = credential.User.Person;
                    user = credential.User;

                    if (person != null) person.IsDeleted = false;
                }

                //make sure the user is part of the account
                var accountAssociation = dc.AccountAssociations.FirstOrDefault(aa => aa.PersonID == user.Guid && aa.AccountID == organizationalUnit.AccountID);

                if (accountAssociation == null)
                {
                    accountAssociation = new AccountAssociation
                    {
                        AccountID = organizationalUnit.AccountID,
                        PersonID = person.Guid
                    };
                    dc.AccountAssociations.Add(accountAssociation);
                }
                else
                {
                    accountAssociation.IsDeleted = false;
                }

                accountID = accountAssociation.AccountID;

                //check to see if the user is already part of the OU
                var organizationalUnitAssociation = dc.OUAssociations.FirstOrDefault(oua => oua.PersonID == person.Guid && oua.OUID == userInvitation.OUID);
                if (organizationalUnitAssociation != null)
                {
                    string reason = "User is already a member of the Organization OU.";
                    PreconditionFailedFault f = new PreconditionFailedFault(102, reason);
                    throw new FaultException<PreconditionFailedFault>(f, reason);
                }

                var role = dc.OURoles.FirstOrDefault(r => r.Guid == userInvitation.RoleID);

                //add the OU association and the Role to that Association
                organizationalUnitAssociation = new OUAssociation
                {
                    OUID = userInvitation.OUID,
                    PersonID = person.Guid,
                    OURoleID = userInvitation.RoleID,
                    RoleType = role.RoleType
                };
                dc.OUAssociations.Add(organizationalUnitAssociation);

                //lastly mark the Invitation as Accepted
                userInvitation.DateAccepted = System.DateTime.UtcNow;
                // Link the UserInvitation to the newly created Person
                userInvitation.ToPersonID = person.Guid;

                // push this data to PowerBI for analytics.
                var pbiNewUser = new PBI_NewUser
                {
                    UserID = person.Guid.ToString(),
                    UserEmail = person.EmailAddressString,
                    OUID = userInvitation.OUID.ToString(),
                    OUName = userInvitation.OU?.Name,
                    RoleID = userInvitation.RoleID.ToString(),
                    InvitationID = userInvitation.Guid.ToString(),
                    InviterID = userInvitation.FromPersonID.ToString(),
                    JoinDate = DateTime.UtcNow,
                    InvitationDate = userInvitation.DateCreated
                };

                _powerBIBridge.Value.PushDataAsync(pbiNewUser);

                try
                {
                    // Your code...
                    // Could also be before try if you know the exception occurs in SaveChanges

                    dc.SaveChanges();

                    if (photo != null)
                    {
                        _blobService.Value.Upload(person.Guid, new BlobModel { Content = photo, ContentType = "image/png" });
                    }

                    //  Register user into MailChimp if he is not already
                    try
                    {
                        _mailChimpAdapter.Value.RegisterUser(userInvitation.EmailAddress);
                    }
                    catch { }

                    //get the smartboardId for the user. We can use any apikey
                    var ouSetting = dc.OUSettings.FirstOrDefault(x => !x.IsDeleted && x.Name == SolarTrackerResources.SelectedSettingName);
                    if (ouSetting != null)
                    {
                        //the method also updates the Ignite user's SmartBoardId property
                        _sbAdapter.Value.GetSBToken(ouSetting.OUID);
                    }

                    if (!string.IsNullOrWhiteSpace(InvitationAcceptRecipient))
                    {
                        var body = $"User: \"{person.Name} <{person.EmailAddressString}>\" accepted invitation for \"{userInvitation.OU?.Name}\"";
                        TM.Mail.Library.SendEmail(InvitationAcceptRecipient, null, $"[{TM.Mail.Library.SenderName}] {person.Name} accepted invitation for {userInvitation.OU?.Name}", body);
                    }
                }
                catch (DbEntityValidationException e)
                {
                    logger.Error("Create User", e);

                    foreach (var eve in e.EntityValidationErrors)
                    {
                        foreach (var ve in eve.ValidationErrors)
                        {
                            Console.WriteLine("- Property: \"{0}\", Value: \"{1}\", Error: \"{2}\"",
                                ve.PropertyName,
                                eve.Entry.CurrentValues.GetValue<object>(ve.PropertyName),
                                ve.ErrorMessage);
                        }
                    }
                    throw;
                }

                var authenticationToken = new AuthenticationToken
                {
                    Audience = "tm",
                    AccountID = accountID,
                    ClientSecret = "asdfjkl;qweruipo",
                    Expiration = System.DateTime.UtcNow.AddDays(TOKEN_EXPIRATION_DAYS).ToUnixTime(),
                    UserID = user.Guid
                };
                return authenticationToken;
            }
        }

        public bool IsUserActive(Guid userId)
        {
            using (var dc = new DataContext())
            {
                // I have no idea why we have the IsDisabled and IsActive flags besides IsDeleted... but checking them just to be safe...
                return dc.Users.Any(u => u.Guid == userId && u.IsDeleted == false && u.IsDisabled == false && u.IsActive == true);
            }
        }

        public string GetCurrentUserFullName()
        {
            using (var ctx = new DataContext())
            {
                return ctx.People.FirstOrDefault(p => p.Guid == SmartPrincipal.UserId)?.Name;
            }
        }

        public SaveResult CreateUpdateUserFromSB(CreateUserDTO newUser, string[] apikey)
        {
            using (DataContext dc = new DataContext())
            {
                var isExist = dc.People.FirstOrDefault(cc => cc.SmartBoardID == newUser.ID);
                if (isExist != null)
                {
                    using (var transaction = dc.Database.BeginTransaction())
                    {
                        try
                        {
                            if (isExist.IsDeleted == true)
                            {
                                return new SaveResult { Success = false, SuccessMessage = "Please activate user" };
                            }

                            if (!String.IsNullOrEmpty(newUser.FirstName))
                            {
                                isExist.FirstName = newUser.FirstName;
                            }

                            if (!String.IsNullOrEmpty(newUser.LastName))
                            {
                                isExist.LastName = newUser.LastName;
                            }

                            if (!String.IsNullOrEmpty(newUser.FirstName) && !String.IsNullOrEmpty(newUser.LastName))
                            {
                                isExist.Name = string.Format("{0} {1}", newUser.FirstName, newUser.LastName);
                            }

                            if (!String.IsNullOrEmpty(newUser.EmailAddress))
                            {
                                isExist.EmailAddressString = newUser.EmailAddress;
                            }

                            if (!String.IsNullOrEmpty(newUser.PhoneNumber))
                            {
                                var isExistPhoneNumber = dc.PhoneNumbers.FirstOrDefault(cc => cc.PersonID == isExist.Guid);
                                if (isExistPhoneNumber != null)
                                {
                                    isExistPhoneNumber.Number = newUser.PhoneNumber;
                                }
                            }

                            if (!String.IsNullOrEmpty(newUser.Password))
                            {
                                var isExistCredentials = dc.Credentials.FirstOrDefault(cc => cc.PersonID == isExist.Guid);
                                if (isExistCredentials != null)
                                {
                                    isExistCredentials.UserName = newUser.EmailAddress;
                                    isExistCredentials.PasswordRaw = newUser.Password;
                                    isExistCredentials.PerformHash();
                                }
                            }

                            var OUAssociations = dc.OUAssociations.Where(oua => oua.PersonID == isExist.Guid);
                            dc.OUAssociations.RemoveRange(OUAssociations);

                            string not_avail = "";

                            foreach (var item in apikey)
                            {
                                var ouSetting = dc.OUSettings.Where(x => x.Name == SolarTrackerResources.SelectedSettingName).ToList()
                              .FirstOrDefault(x =>
                              {
                                  var selectedIntegrations = x.GetValue<ICollection<SelectedIntegrationOption>>();
                                  return selectedIntegrations.Any(s => s?.Data?.SMARTBoard?.ApiKey == item);
                              });

                                if (ouSetting == null)
                                {
                                    not_avail += item + ",";
                                }
                                else
                                {
                                    //check to see if the user is already part of the OU
                                    var organizationalUnitAssociation = dc.OUAssociations.FirstOrDefault(oua => oua.PersonID == isExist.Guid && oua.OUID == ouSetting.OUID);
                                    if (organizationalUnitAssociation == null)
                                    {
                                        var Ou = dc.OUs.FirstOrDefault(x => x.Guid == ouSetting.OUID);
                                        if (Ou != null)
                                        {
                                            var role = dc.OURoles.FirstOrDefault(r => r.Guid == newUser.RoleID);

                                            //add the OU association and the Role to that Association
                                            organizationalUnitAssociation = new OUAssociation
                                            {
                                                OUID = ouSetting.OUID,
                                                PersonID = isExist.Guid,
                                                OURoleID = newUser.RoleID,
                                                RoleType = role.RoleType
                                            };
                                            dc.OUAssociations.Add(organizationalUnitAssociation);
                                        }
                                    }
                                }
                            }

                            dc.SaveChanges();
                            transaction.Commit();
                            if (!String.IsNullOrEmpty(not_avail))
                            {
                                not_avail = not_avail.TrimEnd(',');
                            }

                            return new SaveResult { Success = true, SuccessMessage = "User updated successfully", Exception = not_avail };
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            throw ex;
                        }
                    }
                }
                else 
                {
                    using (var transaction = dc.Database.BeginTransaction())
                    {
                        try
                        {
                            //see if a user exists for this emaiAddress
                            var credential = dc.Credentials
                                        .Include(cred => cred.User)
                                        .Include(cred => cred.User.Person)
                                        .FirstOrDefault(cc => cc.UserName == newUser.EmailAddress);

                            Person person = null;
                            User user = null;
                            Guid accountID = System.Guid.Empty;

                            if (credential == null)
                            {
                                person = new Person
                                {
                                    Guid = Guid.NewGuid(),
                                    FirstName = newUser.FirstName,
                                    LastName = newUser.LastName,
                                    EmailAddressString = newUser.EmailAddress,
                                    SmartBoardID = newUser.ID,
                                    Name = string.Format("{0} {1}", newUser.FirstName, newUser.LastName)
                                };

                                if (!string.IsNullOrEmpty(newUser.PhoneNumber))
                                {
                                    person.PhoneNumbers = new List<PhoneNumber> { new PhoneNumber
                        {
                            PersonID = person.Guid,
                            Number = newUser.PhoneNumber,
                            PhoneType = PhoneType.Mobile
                        }};
                                }
                                dc.People.Add(person);

                                user = new User
                                {
                                    Guid = person.Guid,
                                    PersonID = person.Guid,
                                    Person = person,
                                    NumberOfDevicesAllowed = MaxNumberOfDevicesPerUser
                                };
                                dc.Users.Add(user);

                                var tokenLedger = new TokenLedger
                                {
                                    Name = person.Name,
                                    UserID = person.Guid,
                                    PersonID = person.Guid,
                                    IsPrimary = true
                                };
                                dc.TokenLedgers.Add(tokenLedger);

                                credential = new Credential
                                {
                                    UserName = newUser.EmailAddress,
                                    PasswordRaw = newUser.Password,
                                    UserID = person.Guid,
                                    PersonID = person.Guid,
                                };
                                credential.PerformHash();
                                dc.Credentials.Add(credential);

                            }
                            else
                            {
                                person = credential.User.Person;
                                user = credential.User;

                                if (person != null) person.IsDeleted = false;
                            }

                            string not_avail = "";
                            foreach (var item in apikey)
                            {
                                var ouSetting = dc.OUSettings.Where(x => x.Name == SolarTrackerResources.SelectedSettingName).ToList()
                              .FirstOrDefault(x =>
                              {
                                  var selectedIntegrations = x.GetValue<ICollection<SelectedIntegrationOption>>();
                                  return selectedIntegrations.Any(s => s?.Data?.SMARTBoard?.ApiKey == item);
                              });

                                if (ouSetting == null)
                                {
                                    not_avail += item + ",";
                                }
                                else
                                {
                                    //check to see if the user is already part of the OU
                                    var organizationalUnitAssociation = dc.OUAssociations.FirstOrDefault(oua => oua.PersonID == person.Guid && oua.OUID == ouSetting.OUID);
                                    if (organizationalUnitAssociation == null)
                                    {
                                        var Ou = dc.OUs.FirstOrDefault(x => x.Guid == ouSetting.OUID);
                                        if (Ou != null)
                                        {
                                            var role = dc.OURoles.FirstOrDefault(r => r.Guid == newUser.RoleID);

                                            //add the OU association and the Role to that Association
                                            organizationalUnitAssociation = new OUAssociation
                                            {
                                                OUID = ouSetting.OUID,
                                                PersonID = person.Guid,
                                                OURoleID = newUser.RoleID,
                                                RoleType = role.RoleType
                                            };
                                            dc.OUAssociations.Add(organizationalUnitAssociation);
                                        }
                                    }
                                }    
                            }

                            try
                            {
                                // Your code...
                                // Could also be before try if you know the exception occurs in SaveChanges

                                dc.SaveChanges();

                                //  Register user into MailChimp if he is not already
                                try
                                {
                                    _mailChimpAdapter.Value.RegisterUser(newUser.EmailAddress);
                                }
                                catch { }
                            }
                            catch (DbEntityValidationException e)
                            {
                                logger.Error("Create User", e);

                                foreach (var eve in e.EntityValidationErrors)
                                {
                                    foreach (var ve in eve.ValidationErrors)
                                    {
                                        Console.WriteLine("- Property: \"{0}\", Value: \"{1}\", Error: \"{2}\"",
                                            ve.PropertyName,
                                            eve.Entry.CurrentValues.GetValue<object>(ve.PropertyName),
                                            ve.ErrorMessage);
                                    }
                                }
                                throw;
                            }

                            var authenticationToken = new AuthenticationToken
                            {
                                UserID = user.Guid
                            };

                            transaction.Commit();

                            if (!String.IsNullOrEmpty(not_avail))
                            {
                                not_avail = not_avail.TrimEnd(',');
                            }

                            return new SaveResult { Success = true, SuccessMessage = "User created successfully", Exception = not_avail };
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            throw ex;
                        }
                    }
                }
            }
        }
    }
}
