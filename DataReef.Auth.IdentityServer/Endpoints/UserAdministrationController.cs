using System;
using System.Collections.Generic;
using System.Net;
using System.Linq;
using System.Web.Http;
using Newtonsoft.Json.Linq;
using Thinktecture.IdentityServer.Core.Logging;
using DataReef.Auth.IdentityServer.Helpers;
using DataReef.Auth.Core.Services;
using DataReef.Auth.Core.Models;
using DataReef.Auth.IdentityServer.Helpers.Exceptions;
using DataReef.Auth.IdentityServer.Services;

namespace DataReef.Auth.IdentityServer.Endpoints
{
    public class UserAdministrationController : ApiController
    {
        private const string UserIdFieldKey = "userid";
        private const string OldPasswordFieldKey = "oldpassword";
        private const string NewPasswordFieldKey = "newpassword";
        private const string CredentialTypeFieldKey = "credentialtype";

        private IUserSelfAdministrationService _userProfileService;
        private ILog _logger;

        public UserAdministrationController()
        {
            _userProfileService = new UserAdministrationService();
            _logger = LogProvider.GetLogger("SmartLogger");
        }

        public UserAdministrationController(IUserSelfAdministrationService userProfileService)
        {
            _userProfileService = userProfileService;
        }

        [HttpPost]
        public IHttpActionResult ChangePassword(JObject jsonContent)
        {
            IDictionary<string, string> content = jsonContent.ToObject<Dictionary<string, string>>();
            content = content.ToDictionary(k => k.Key.ToLower(), v => v.Value);

            Guid userId;
            if (!content.ContainsKey(UserIdFieldKey) || !Guid.TryParse(content[UserIdFieldKey], out userId))
            {
                return BadRequest(AuthConstants.ErorrMessages.InvalidContent);
            }

            if (!content.ContainsKey(OldPasswordFieldKey)) return BadRequest(AuthConstants.ErorrMessages.InvalidContent);
            string oldPassword = content[OldPasswordFieldKey];
            if (!content.ContainsKey(NewPasswordFieldKey)) return BadRequest(AuthConstants.ErorrMessages.InvalidContent);
            string newPassword = content[NewPasswordFieldKey];

            CredentialType credentialType;
            if (!content.ContainsKey(CredentialTypeFieldKey) ||
                !Enum.TryParse<CredentialType>(content[CredentialTypeFieldKey], true, out credentialType) ||
                !Enum.IsDefined(typeof(CredentialType), credentialType))
            {
                return BadRequest(AuthConstants.ErorrMessages.InvalidContent);
            }

            try
            {
                _userProfileService.ChangePassword(userId, oldPassword, newPassword, credentialType);
            }
            catch (ArgumentException ex)
            {
                _logger.ErrorException(string.Format("Cannot change password for user with GUID={0} because the user cannot be found.", userId), ex);
                return JsonHelper.JsonResult(HttpStatusCode.NotFound, new { Message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.ErrorException(string.Format("Cannot change password for user with GUID={0} because the user is disabled.", userId), ex);
                return JsonHelper.JsonResult(HttpStatusCode.Forbidden, new { Message = ex.Message });
            }
            catch (NotAuthorizedException ex)
            {
                _logger.ErrorException(string.Format("Not authorized request to change password for user with GUID={0}.", userId), ex);
                return JsonHelper.JsonResult(HttpStatusCode.Unauthorized, new { Message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                _logger.ErrorException(string.Format("Invalid request to change password for user with GUID={0}.", userId), ex);
                return JsonHelper.JsonResult(HttpStatusCode.NotAcceptable, new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.ErrorException(string.Format("An error occured while trying to set the new password for user with GUID={0}.", userId), ex);
                return InternalServerError();
            }

            return Json(new { Message = AuthConstants.SuccessMessages.PasswordChanged });
        }
    }
}