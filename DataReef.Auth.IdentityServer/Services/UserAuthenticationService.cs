using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using Microsoft.Owin;
using Thinktecture.IdentityServer.Core.Authentication;
using Thinktecture.IdentityServer.Core.Models;
using Thinktecture.IdentityServer.Core.Services;
using Thinktecture.IdentityServer.Core;
using DataReef.Auth.IdentityServer.DataAccess;
using DataReef.Auth.Core.Models;
using DataReef.Auth.IdentityServer.Helpers;
using Newtonsoft.Json;

namespace DataReef.Auth.IdentityServer.Services
{
    class UserAuthenticationService : IUserService
    {
        DataContext _dataContext;

        public UserAuthenticationService(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public Task<AuthenticateResult> AuthenticateExternalAsync(ExternalIdentity externalUser)
        {
            return Task.FromResult<AuthenticateResult>(null);
        }

        public Task<AuthenticateResult> AuthenticateLocalAsync(string username, string password, SignInMessage message = null)
        {
            Credential credential = _dataContext.Credentials.FirstOrDefault(c => c.Username.Equals(username, StringComparison.CurrentCultureIgnoreCase));
            if (credential == null)
            {
                // Wrong username
                HttpHelper.StashResponseDetails(HttpStatusCode.Unauthorized, AuthConstants.ErorrMessages.InvalidCredentials);
                return Task.FromResult<AuthenticateResult>(null);
            }

            if (credential.Password != CryptographyHelper.ComputePasswordHash(password, credential.Salt))
            {
                // Wrong password
                HttpHelper.StashResponseDetails(HttpStatusCode.Unauthorized, AuthConstants.ErorrMessages.InvalidCredentials);
                return Task.FromResult<AuthenticateResult>(null);
            }

            if (credential.RequirePasswordChange)
            {
                // User has to change his password before getting access
                HttpHelper.StashResponseDetails(HttpStatusCode.PreconditionFailed, AuthConstants.ErorrMessages.PasswordChangeRequired);
                return Task.FromResult<AuthenticateResult>(null);
            }

            IList<User> users = credential.Users;
            if (users == null || users.Count == 0)
            {
                // No user associated with the credential
                // This should never hapen because users are created at the same time with the credentials and they are never removed (just disabled)
                HttpHelper.StashResponseDetails(HttpStatusCode.Unauthorized, AuthConstants.ErorrMessages.UserDoesNotExist);
                return Task.FromResult<AuthenticateResult>(null);
            }

            User selectedUser = null;
            if (HttpContext.Current == null)
            {
                // This should never hapen for HTTP requests
                HttpHelper.StashResponseDetails(HttpStatusCode.Unauthorized, AuthConstants.ErorrMessages.UnknownError);
                return Task.FromResult<AuthenticateResult>(null);
            }

            // Check whether the client has provided the OU it requires access in the request header
            IOwinContext context = HttpContext.Current.GetOwinContext();
            if (context.Request.Headers.ContainsKey(AuthConstants.OuIdHeaderKey))
            {
                Guid ouId;
                if (Guid.TryParse(context.Request.Headers[AuthConstants.OuIdHeaderKey], out ouId))
                {
                    selectedUser = users.FirstOrDefault(u => u.OuId == ouId);
                    if (selectedUser == null)
                    {
                        // User not registered with the requested OU
                        HttpHelper.StashResponseDetails(HttpStatusCode.Forbidden, AuthConstants.ErorrMessages.UserNotRegistered);
                        return Task.FromResult<AuthenticateResult>(null);
                    }
                }
                else
                {
                    // Invalid OU id provided
                    HttpHelper.StashResponseDetails(HttpStatusCode.BadRequest, AuthConstants.ErorrMessages.InvalidOuId);
                    return Task.FromResult<AuthenticateResult>(null);
                }
            }
            else
            {
                IList<User> activeUsers = users.Where(u => u.IsActive).ToList();
                // When many users are associated with a credential, the client needs to specify the OU it wants access to
                if (activeUsers.Count > 1)
                {
                    // No OU specified: force client to resubmit th request
                    // The result will also contain a list of available OUs so that the client can pick one
                    IDictionary<string, string> usersOuData = activeUsers.ToDictionary(k => k.OuId.ToString(), v => v.OuName);
                    HttpHelper.StashResponseDetails(HttpStatusCode.Conflict, AuthConstants.ErorrMessages.UserConflict, usersOuData);
                    return Task.FromResult<AuthenticateResult>(null);
                }
                else if (activeUsers.Count == 1)
                {
                    selectedUser = activeUsers.First();
                }
                else
                {
                    // User is disabled
                    HttpHelper.StashResponseDetails(HttpStatusCode.Forbidden, AuthConstants.ErorrMessages.UserDisabled);
                    return Task.FromResult<AuthenticateResult>(null);
                }
            }

            IList<Claim> userClaims = new List<Claim>();
            userClaims.Add(new Claim(Constants.ClaimTypes.Subject, selectedUser.UserId.ToString()));
            userClaims.Add(new Claim(Constants.ClaimTypes.Name, credential.Username));
            userClaims.Add(new Claim(Constants.ClaimTypes.AuthenticationMethod, Constants.AuthenticationMethods.Password));
            userClaims.Add(new Claim(Constants.ClaimTypes.IdentityProvider, Constants.BuiltInIdentityProvider));
            userClaims.Add(new Claim(Constants.ClaimTypes.AuthenticationTime, DateTime.UtcNow.ToEpochTime().ToString(), ClaimValueTypes.Integer64));
            userClaims.Add(new Claim(AuthConstants.TenantIdClaimType, selectedUser.TenantId.ToString()));

            ClaimsIdentity identity = new ClaimsIdentity(userClaims, Constants.PrimaryAuthenticationType);
            ClaimsPrincipal principal = new ClaimsPrincipal(identity);

            return Task.FromResult(new AuthenticateResult(principal));
        }

        public Task<IEnumerable<Claim>> GetProfileDataAsync(ClaimsPrincipal subject, IEnumerable<string> requestedClaimTypes = null)
        {
            return Task.FromResult<IEnumerable<Claim>>(null);
        }

        public Task<bool> IsActiveAsync(ClaimsPrincipal subject)
        {
            Claim userIdClaim = subject.Claims.FirstOrDefault(c => c.Type == Constants.ClaimTypes.Subject);
            if (userIdClaim == null)
            {
                return Task.FromResult<bool>(false);
            }

            Guid userGuid;
            if (!Guid.TryParse(userIdClaim.Value, out userGuid))
            {
                return Task.FromResult<bool>(false);
            }

            User user = _dataContext.Users.FirstOrDefault(u => u.UserId == userGuid);
            if (user == null)
            {
                return Task.FromResult<bool>(false);
            }

            return Task.FromResult<bool>(user.IsActive);
        }
    }
}