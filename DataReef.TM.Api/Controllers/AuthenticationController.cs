using DataReef.Core;
using DataReef.Core.Classes;
using DataReef.Core.Infrastructure.Authorization;
using DataReef.TM.Api.Areas.HelpPage.Extensions;
using DataReef.TM.Api.Bootstrap;
using DataReef.TM.Api.Classes;
using DataReef.TM.Classes;
using DataReef.TM.Contracts.Auth;
using DataReef.TM.Contracts.FaultContracts;
using DataReef.TM.Contracts.Services;
using DataReef.TM.Models;
using DataReef.TM.Models.DataViews;
using DataReef.TM.Models.DTOs.Persons;
using Newtonsoft.Json;
using System;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description; 

namespace DataReef.TM.Api.Controllers
{
    /// <summary>
    /// Use this controller for all authentication request.. creating a user, resetting password. etc.  this controller does not require an auth header for some actions
    /// </summary>
    [RoutePrefix("api/v1/authentication")]
    public class AuthenticationController : ApiController
    {
        private readonly IAuthenticationService authService;
        private readonly IDataService<Credential> credentialService;
        private readonly Lazy<IUserInvitationService> _userInvitationService;

        public AuthenticationController(IAuthenticationService authService,
            IDataService<Credential> credentialService,
            Lazy<IUserInvitationService> userInvitationService)
        {
            this.authService = authService;
            this.credentialService = credentialService;
            this._userInvitationService = userInvitationService;
        }

        /// <summary>
        /// Get Hash by old password and salt
        /// </summary>
        /// <param name="oldpwd"></param>
        /// <param name="salt"></param>
        /// <returns></returns>
        [Route("GetHash")]
        [HttpGet]
        [AllowAnonymous]
        public async Task<IHttpActionResult> GetHash(string oldpwd, string salt)
        {
            try
            {
                string hash = DataReef.Auth.Helpers.CryptographyHelper.ComputePasswordHash(oldpwd, salt);
                return Ok(hash);
            }
            catch (ArgumentException ex)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Unauthorized) { Content = new StringContent(ex.Message) });
            }
        }

        /// <summary>
        /// Authenticates UserName and Password and returns an Authentication Token
        /// </summary>
        /// <param name="post"></param>
        /// <returns></returns>
        [Route("")]
        [HttpPost]
        [AllowAnonymous]
        public async Task<IHttpActionResult> Authenticate(AuthenticationPost post)
        {
            try
            {
                if (post == null)
                {
                    throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.PreconditionFailed));
                }

                AuthenticationToken token = authService.Authenticate(post.UserName, post.Password, post.fcm_token);
                Jwt ret = this.EncodeToken(token);
                return Ok<Jwt>(ret);
            }
            catch (ArgumentException ex)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Unauthorized) { Content = new StringContent(ex.Message) });
            }
        }


        /// <summary>
        /// Authenticates UserName and Password and returns an Authentication Token
        /// </summary>
        /// <param name="personid"></param>
        /// <returns></returns>
        [Route("SuperAdm/{personid}")]
        [HttpPost]
        // [AllowAnonymous]
        public async Task<IHttpActionResult> AuthUserBySuperAdmin(Guid personid)
        {
            try
            {
                if (personid == null)
                {
                    throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.PreconditionFailed));
                }

                AuthenticationToken token = authService.AuthenticateUserBySuperAdmin(personid);
                Jwt ret = this.EncodeToken(token);
                return Ok<Jwt>(ret);
            }
            catch (ArgumentException ex)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Unauthorized) { Content = new StringContent(ex.Message) });
            }
        }
        /// <summary>
        ///Update User Status
        /// </summary>
        [Route("UserStatus/{userId}/{value}")]
        [HttpGet]
        // [AllowAnonymous]
        public async Task<IHttpActionResult> UpdateUserStatus(bool value, Guid userId)
        {
            var status = authService.updateUser(value, userId);
            return Ok(status);
        }



        /// <summary>
        /// Creates a new user and returns an authentication token
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [GenericRoute("acceptinvitation")]
        [HttpPost]
        [AllowAnonymous]
        public async Task<IHttpActionResult> AcceptInvitation(NewUser user)
        {
            try
            {
                if (user == null)
                {
                    throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.PreconditionFailed));
                }

                AuthenticationToken token = authService.CreateUser(user);
                if (token?.isAlreadyMember == true)
                { 
                    return Redirect(PathExtensions.ToAbsoluteUrl("/home/AlreadyAccepted"));
                }
                Jwt ret = this.EncodeToken(token);
                return Ok<Jwt>(ret);
            }
            catch (System.ServiceModel.FaultException fe)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Conflict) { Content = new StringContent(fe.Message) });
            }
            catch (ResourceExistsException ree)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Conflict) { Content = new StringContent(ree.Message) });
            }
            catch (Exception ex)
            {
                throw;

                //throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError));
            }

        }

        /// <summary>
        /// Creates a new user and returns an authentication token
        /// </summary>
        /// <param name="user"></param>
        /// <param name="apiKey"></param>
        /// <returns></returns>
        [GenericRoute("createuser/{apiKey}")]
        [HttpPost]
        [AllowAnonymous]
        public async Task<IHttpActionResult> CreateUserFromSmartBoard([FromBody] CreateUserDTO user, string apiKey)
        {
            try
            {
                if (user == null)
                {
                    throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.PreconditionFailed));
                }

                var createdUserInvitation = this._userInvitationService.Value.SilentInsertFromSmartboard(user, apiKey);
                if (createdUserInvitation == null)
                {
                    throw new Exception("Could not create the user invitation");
                }

                var userCreator = new NewUser
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    InvitationGuid = createdUserInvitation.Guid,
                    Password = user.Password
                };

                authService.CreateUser(userCreator);
                return Ok(true);
            }
            catch (System.ServiceModel.FaultException fe)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Conflict) { Content = new StringContent(fe.Message) });
            }
            catch (ResourceExistsException ree)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Conflict) { Content = new StringContent(ree.Message) });
            }
            catch (Exception ex)
            {
                throw;
                //throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError));
            }

        }

        /// <summary>
        /// Using the reset guid sent in the email, accepts a new password and completes the reset process
        /// </summary>
        /// <param name="completionObject"></param>
        /// <returns>AuthenticationToken</returns>
        [GenericRoute("reset/complete")]
        [HttpPost]
        [AllowAnonymous]
        public async Task<IHttpActionResult> CompleteResetPassword(PasswordResetCompletion completionObject)
        {
            try
            {
                AuthenticationToken token = authService.CompletePasswordReset(completionObject.ResetGuid, completionObject.NewPassword, completionObject.fcm_token);
                Jwt ret = this.EncodeToken(token);
                return Ok<Jwt>(ret);

            }
            catch (System.ServiceModel.FaultException fe)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Conflict));
            }
            catch (ResourceExistsException)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Conflict));
            }
            catch (Exception ex)
            {
                throw;
                //throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError));
            }

        }


        [GenericRoute("changepassword")]
        [HttpPut]
        [AllowAnonymous]
        public async Task<IHttpActionResult> ChangePassword(PasswordChange change)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    AuthenticationToken token = authService.ChangePassword(change.UserName, change.OldPassword, change.NewPassword, change.fcm_token);
                    Jwt ret = this.EncodeToken(token);
                    return Ok<Jwt>(ret);
                }
                else
                {
                    var errors = ModelState.Where(ms => ms.Value.Errors.Count > 0).SelectMany(ms => ms.Value.Errors.Select(e => e.ErrorMessage)).FirstOrDefault();
                     throw new ApplicationException(errors);
                    //throw new HttpResponseException(new HttpResponseMessage() { StatusCode = HttpStatusCode.NotFound, ReasonPhrase = errors });
                }
            }
            catch (System.ServiceModel.FaultException fe)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Conflict));
            }
            catch (ResourceExistsException)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Conflict));
            }
            catch (Exception ex)
            {
                throw;
                //throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError));
            }

        }


        /// <summary>
        ///  Inititiates a PassReset that is completed via email
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [GenericRoute("reset/initiate")]
        [HttpPost]
        [AllowAnonymous]
        public async Task<IHttpActionResult> ResetPassword(PasswordReset resetObject)
        {
            try
            {
                resetObject.ExpirationDate = System.DateTime.UtcNow.AddDays(2);
                var saveResult = authService.InitiatePasswordReset(resetObject);
                if (saveResult.Success)
                {
                    return Ok();
                }

                return ResponseMessage(new HttpResponseMessage(HttpStatusCode.Gone) { Content = new StringContent(saveResult.ExceptionMessage) });
            }
            catch (System.ServiceModel.FaultException)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Conflict));
            }
            catch (ResourceExistsException)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Conflict));
            }
            catch (Exception)
            {
                throw;
                //throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError));
            }
        }

        [GenericRoute("token/extend")]
        [HttpPost]
        [AllowAnonymous]
        public async Task<IHttpActionResult> ExtendTokenValidity([FromBody] string token)
        {
            var certificate = DataReef.TM.Api.Security.Certificates.Certificate.Get();
            var authToken = AuthenticationToken.FromEncryptedString(token, certificate);
            authToken.Expiration = DateTime.UtcNow.AddDays(7).ToUnixTime();
            return Ok<Jwt>(EncodeToken(authToken));
        }

        /// <summary>
        /// Creates a new user from smart board
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost, Route("createuser/smartboard")]
        [AllowAnonymous, InjectAuthPrincipal]
        [ResponseType(typeof(SaveResult))]
        public SaveResult CreateUserFromSB([FromBody] CreateUserDTO user)
        {
            if (user == null)
            {
                return new SaveResult { Success = false, ExceptionMessage = "request data can not null" };
            }

            if (ModelState.IsValid)
            {
                var ret = authService.CreateUpdateUserFromSB(user, user.apikey);
                return ret;
            }
            else
            {
                var errors = ModelState.Where(ms => ms.Value.Errors.Count > 0).SelectMany(ms => ms.Value.Errors.Select(e => e.ErrorMessage)).FirstOrDefault();
                return new SaveResult { Success = false, ExceptionMessage = errors };
            }

        }


        #region Private

        private Jwt EncodeToken(AuthenticationToken token)
        {

            string tokenJson = JsonConvert.SerializeObject(token);
            var cert = DataReef.TM.Api.Security.Certificates.Certificate.Get();
            RSACryptoServiceProvider rsa = cert.PublicKey.Key as RSACryptoServiceProvider;
            byte[] cryptedData = rsa.Encrypt(System.Text.UTF8Encoding.UTF8.GetBytes(tokenJson), true);
            string tokenString = Convert.ToBase64String(cryptedData);

            return new Jwt
            {
                Expiration = token.Expiration,
                Token = tokenString
            };
        }

        #endregion
    }
}
