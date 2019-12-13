using System;
using System.Collections.Generic;
using System.Linq;
using Thinktecture.IdentityServer.Core.Logging;
using DataReef.Auth.IdentityServer.DataAccess;
using DataReef.Auth.Core.Models;
using DataReef.Auth.IdentityServer.Helpers;
using DataReef.Auth.IdentityServer.Helpers.Exceptions;
using DataReef.Auth.Core.Services;
using System.ServiceModel;

namespace DataReef.Auth.IdentityServer.Endpoints
{
    public class UserAdministrationService : IUserAdministrationService, IUserSelfAdministrationService
    {
        private DataContext _dataContext;
        private ILog _logger;

        public UserAdministrationService()
        {
            _dataContext = new DataContext();
            _logger = LogProvider.GetLogger("SmartLogger");
        }

        public void CreateUser(string username, CredentialType credentialType, Guid userId, Guid ouId, string ouName, int tenantId, bool requirePasswordChange = true)
        {
            if (string.IsNullOrEmpty(username)) throw new FaultException(AuthConstants.ErorrMessages.NullUsername);

            // Check whether the user already exists in the system
            User user = _dataContext.Users.FirstOrDefault(c => c.UserId == userId);
            if (user != null)
            {
                // If the user exists, a new credential has to be added for the existing user
                // Check whether the same user is not trying to register with another organization
                if (user.OuId != ouId)
                {
                    _logger.ErrorFormat("The user '{0}' is already registered with the organization '{1}'.", user.UserId, user.OuId);
                    throw new FaultException(AuthConstants.ErorrMessages.UserAlreadyRegistered);
                }

                // Check whether the requested username does not already exists
                Credential credential = _dataContext.Credentials.FirstOrDefault(c => c.Username == username);
                if (credential != null)
                {
                    throw new FaultException(AuthConstants.ErorrMessages.CredentialAlreadyExists);
                }

                // Check whether the user does not already have a credential of the requested type
                credential = user.Credentials.FirstOrDefault(c => c.Type == credentialType);
                if (credential != null)
                {
                    throw new FaultException(AuthConstants.ErorrMessages.UserCredentialAlreadyExists);
                }

                // Create the new credential for the user
                credential = new Credential()
                {
                    Username = username,
                    Type = credentialType,
                    RequirePasswordChange = requirePasswordChange
                };

                credential.Salt = CryptographyHelper.GenerateSalt();
                string password = CryptographyHelper.GeneratePassword(5, 2);
                credential.Password = CryptographyHelper.ComputePasswordHash(password, credential.Salt);
                user.Credentials.Add(credential);

                //TODO: place work item to queue 
            }
            else
            {
                // Create the new user
                user = new User()
                {
                    UserId = userId,
                    OuId = ouId,
                    OuName = ouName,
                    TenantId = tenantId
                };
                _dataContext.Users.Add(user);

                // Check whether a credential already exists for the new user
                Credential credential = _dataContext.Credentials.FirstOrDefault(c => c.Username == username);
                if (credential == null)
                {
                    credential = new Credential()
                    {
                        Username = username,
                        Type = credentialType,
                        RequirePasswordChange = requirePasswordChange
                    };

                    credential.Salt = CryptographyHelper.GenerateSalt();
                    string password = CryptographyHelper.GeneratePassword(5, 2);
                    credential.Password = CryptographyHelper.ComputePasswordHash(password, credential.Salt);

                    user.Credentials = new List<Credential>();
                    user.Credentials.Add(credential);
                }
                else
                {
                    credential.Users.Add(user);
                }
                
                //TODO: place work item to queue 
            }

            try
            {
                _dataContext.SaveChanges();
            }
            catch (Exception ex)
            {
                _logger.ErrorException(string.Format("An error occured while trying to create the new user with GUID={0}.", userId), ex);
                throw new FaultException(AuthConstants.ErorrMessages.UnknownError);
            }

            //TODO: place work item to queue
        }

        public void DisableUser(Guid userId)
        {
            User targetUser = _dataContext.Users.FirstOrDefault(o => o.UserId == userId);
            if (targetUser == null)
            {
                throw new FaultException(AuthConstants.ErorrMessages.InvalidUserId);
            }

            targetUser.IsActive = false;

            try
            {
                _dataContext.SaveChanges();
            }
            catch (Exception ex)
            {
                _logger.ErrorException(string.Format("An error occured while trying to disable the user with GUID={0}.", userId), ex);
                throw new FaultException(AuthConstants.ErorrMessages.UnknownError);
            }

            //TODO: place work item to queue
        }

        public void DisableUsers(IList<Guid> userIds)
        {
            foreach (User user in _dataContext.Users.Where(u => userIds.Contains(u.UserId)))
            {
                user.IsActive = false;
            }

            try
            {
                _dataContext.SaveChanges();
            }
            catch (Exception ex)
            {
                _logger.ErrorException(string.Format("An error occured while trying to disable the users with GUIDs={0}.", string.Join(",", userIds)), ex);
                throw new Exception(AuthConstants.ErorrMessages.UnknownError);
            }

            //TODO: place work into queue
        }

        public void EnableUser(Guid userId)
        {
            User targetUser = _dataContext.Users.FirstOrDefault(u => u.UserId == userId);
            if (targetUser == null)
            {
                throw new FaultException(AuthConstants.ErorrMessages.InvalidUserId);
            }

            targetUser.IsActive = true;

            try
            {
                _dataContext.SaveChanges();
            }
            catch (Exception ex)
            {
                _logger.ErrorException(string.Format("An error occured while trying to enable the user with GUID={0}.", userId), ex);
                throw new FaultException(AuthConstants.ErorrMessages.UnknownError);
            }

            //TODO: place work into queue
        }

        public void EnableUsers(IList<Guid> userIds)
        {
            foreach (User user in _dataContext.Users.Where(u => userIds.Contains(u.UserId)))
            {
                user.IsActive = true;
            }

            try
            {
                _dataContext.SaveChanges();
            }
            catch (Exception ex)
            {
                _logger.ErrorException(string.Format("An error occured while trying to enable the users with GUIDs={0}.", string.Join(",", userIds)), ex);
                throw new Exception(AuthConstants.ErorrMessages.UnknownError);
            }

            //TODO: place work into queue
        }

        public void RequestPasswordChange(Guid userId, CredentialType credentialType = CredentialType.All)
        {
            User targetUser = _dataContext.Users.FirstOrDefault(u => u.UserId == userId);
            if (targetUser == null)
            {
                throw new FaultException(AuthConstants.ErorrMessages.InvalidUserId);
            }

            if (credentialType == CredentialType.All)
            {
                foreach (Credential credential in targetUser.Credentials)
                {
                    credential.RequirePasswordChange = true;
                }
            }
            else
            {
                Credential credential = targetUser.Credentials.FirstOrDefault(c => c.Type == credentialType);
                if (credential == null)
                {
                    throw new FaultException(AuthConstants.ErorrMessages.CredentialNotFound);
                }

                credential.RequirePasswordChange = true;
            }

            try
            {
                _dataContext.SaveChanges();
            }
            catch (Exception ex)
            {
                _logger.ErrorException(string.Format("An error occured while trying to request password change for user with GUID={1}.", credentialType, userId), ex);
                throw new FaultException(AuthConstants.ErorrMessages.UnknownError);
            }
        }

        public void RequestPasswordsChange(IList<Guid> userIds, CredentialType credentialType = CredentialType.All)
        {
            foreach (Guid userId in userIds)
            {
                User targetUser = _dataContext.Users.FirstOrDefault(u => u.UserId == userId);
                if (targetUser == null)
                {
                    _logger.ErrorFormat("Could not request password change for user with GUID={0} because it cannot be found.", userId);
                    continue;
                }

                if (credentialType == CredentialType.All)
                {
                    foreach (Credential credential in targetUser.Credentials)
                    {
                        credential.RequirePasswordChange = true;
                    }
                }
                else
                {
                    Credential credential = targetUser.Credentials.FirstOrDefault(c => c.Type == credentialType);
                    if (credential == null)
                    {
                        _logger.ErrorFormat("Could not request password change for user with GUID={1} because it cannot be found.",
                            credentialType, userId);
                    }

                    credential.RequirePasswordChange = true;
                }
            }

            try
            {
                _dataContext.SaveChanges();
            }
            catch (Exception ex)
            {
                _logger.ErrorException(string.Format("An error occured while trying to request password change for {0} credential of users with GUIDs={1}.", credentialType, string.Join(",", userIds)), ex);
                throw new FaultException(AuthConstants.ErorrMessages.UnknownError);
            }
        }

        public void ResetPassword(Guid userId, CredentialType credentialType = CredentialType.All)
        {
            User targetUser = _dataContext.Users.FirstOrDefault(u => u.UserId == userId);
            if (targetUser == null)
            {
                throw new FaultException(AuthConstants.ErorrMessages.InvalidUserId);
            }

            if (credentialType == CredentialType.All)
            {
                foreach (Credential credential in targetUser.Credentials)
                {
                    string password = null;
                    if (credentialType == CredentialType.Pin)
                    {
                        password = CryptographyHelper.GeneratePassword(0, 5);
                    }
                    else
                    {
                        password = CryptographyHelper.GeneratePassword(5, 2);
                    }

                    credential.Password = CryptographyHelper.ComputePasswordHash(password, credential.Salt);
                }
            }
            else
            {
                Credential credential = targetUser.Credentials.FirstOrDefault(c => c.Type == credentialType);
                if (credential == null)
                {
                    throw new FaultException(AuthConstants.ErorrMessages.CredentialNotFound);
                }

                string password = null;
                if (credentialType == CredentialType.Pin)
                {
                    password = CryptographyHelper.GeneratePassword(0, 5);
                }
                else
                {
                    password = CryptographyHelper.GeneratePassword(5, 2);
                }

                credential.Password = CryptographyHelper.ComputePasswordHash(password, credential.Salt);
            }

            try
            {
                _dataContext.SaveChanges();
            }
            catch (Exception ex)
            {
                _logger.ErrorException(string.Format("An error occured while trying to reset the password for user with GUID={0}.", userId), ex);
                throw new FaultException(AuthConstants.ErorrMessages.UnknownError);
            }

            //TODO: place work item into queue
        }

        public void ResetPasswords(IList<Guid> userIds, CredentialType credentialType = CredentialType.All)
        {
            foreach (Guid userId in userIds)
            {
                User targetUser = _dataContext.Users.FirstOrDefault(u => u.UserId == userId);
                if (targetUser == null)
                {
                    _logger.ErrorFormat("Could not reset the password for user with GUID={0} because user cannot be found.", userId);
                    continue;
                }

                if (credentialType == CredentialType.All)
                {
                    foreach (Credential credential in targetUser.Credentials)
                    {
                        string password = null;
                        if (credentialType == CredentialType.Pin)
                        {
                            password = CryptographyHelper.GeneratePassword(0, 5);
                        }
                        else
                        {
                            password = CryptographyHelper.GeneratePassword(5, 2);
                        }

                        credential.Password = CryptographyHelper.ComputePasswordHash(password, credential.Salt);
                    }
                }
                else
                {
                    Credential credential = targetUser.Credentials.FirstOrDefault(c => c.Type == credentialType);
                    if (credential == null)
                    {
                        _logger.ErrorFormat("Could not reset the password for user with GUID={0} because {1} credential it cannot be found.", userId, credentialType);
                        continue;
                    }

                    string password = null;
                    if (credentialType == CredentialType.Pin)
                    {
                        password = CryptographyHelper.GeneratePassword(0, 5);
                    }
                    else
                    {
                        password = CryptographyHelper.GeneratePassword(5, 2);
                    }

                    credential.Password = CryptographyHelper.ComputePasswordHash(password, credential.Salt);
                }
            }

            try
            {
                _dataContext.SaveChanges();
            }
            catch (Exception ex)
            {
                _logger.ErrorException(string.Format("An error occured while trying to reset the password for users with GUIDs={0}.", string.Join(",", userIds)), ex);
                throw new FaultException(AuthConstants.ErorrMessages.UnknownError);
            }

            //TODO: place work item into queue
        }

        public void ChangeUsername(Guid userId, string newUsername, CredentialType credentialType)
        {
            User targetUser = _dataContext.Users.FirstOrDefault(u => u.UserId == userId);
            if (targetUser == null)
            {
                _logger.ErrorFormat("Could not reset the password for user with GUID={0} because user cannot be found.", userId);
                return;
            }

            if (targetUser.IsActive == false)
            {
                throw new FaultException(AuthConstants.ErorrMessages.UserDisabled);
            }

            Credential credential = targetUser.Credentials.FirstOrDefault(c => c.Type == credentialType);
            if (credential == null)
            {
                throw new FaultException(AuthConstants.ErorrMessages.InvalidCredentials);
            }

            credential.Username = newUsername;

            try
            {
                _dataContext.SaveChanges();
            }
            catch (Exception ex)
            {
                _logger.ErrorException(string.Format("An error occured while trying to change the username for user with GUID={0}.", userId), ex);
                throw new FaultException(AuthConstants.ErorrMessages.UnknownError);
            }
        }

        public void ChangePassword(Guid userId, string oldPassword, string newPassword, CredentialType credentialType)
        {
            User targetUser = _dataContext.Users.FirstOrDefault(u => u.UserId == userId);
            if (targetUser == null)
            {
                throw new ArgumentException(AuthConstants.ErorrMessages.UserDoesNotExist);
            }

            if (targetUser.IsActive == false)
            {
                throw new UnauthorizedAccessException(AuthConstants.ErorrMessages.UserDisabled);
            }

            Credential credential = targetUser.Credentials.FirstOrDefault(c => c.Type == credentialType);
            if (credential == null)
            {
                throw new NotAuthorizedException(AuthConstants.ErorrMessages.InvalidCredentials);
            }

            if (credential.Password != CryptographyHelper.ComputePasswordHash(oldPassword, credential.Salt))
            {
                throw new NotAuthorizedException(AuthConstants.ErorrMessages.InvalidCredentials);
            }

            string hashedNewPassword = CryptographyHelper.ComputePasswordHash(newPassword, credential.Salt);
            if (credential.Password == hashedNewPassword)
            {
                throw new InvalidOperationException(AuthConstants.ErorrMessages.InvalidPasswordHistory);
            }

            credential.Password = hashedNewPassword;

            try
            {
                _dataContext.SaveChanges();
            }
            catch
            {
                throw;
            }
        }
    }
}