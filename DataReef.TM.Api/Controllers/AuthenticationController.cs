using DataReef.Core;
using DataReef.TM.Api.Bootstrap;
using DataReef.TM.Api.Classes;
using DataReef.TM.Contracts.Auth;
using DataReef.TM.Contracts.FaultContracts;
using DataReef.TM.Contracts.Services;
using DataReef.TM.Models;
using DataReef.TM.Models.DataViews;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Web.Http;

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

        public AuthenticationController(IAuthenticationService authService,IDataService<Credential> credentialService)
        {
            this.authService = authService;
            this.credentialService = credentialService;
        }

        /// <summary>
        /// Authenticates UserName and Password and returns an Authentication Token
        /// </summary>
        /// <param name="post"></param>
        /// <returns></returns>
        [Route("")]
        [HttpPost]
        [AllowAnonymous]
        public IHttpActionResult Authenticate(AuthenticationPost post)
        {
            try
            {
                if (post == null)
                {
                    throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.PreconditionFailed));
                }

                AuthenticationToken token = authService.Authenticate(post.UserName, post.Password);
                Jwt ret = this.EncodeToken(token);
                return Ok<Jwt>(ret);
            }
            catch (ArgumentException ex)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Unauthorized) { Content = new StringContent(ex.Message) });
            }
        }

        /// <summary>
        /// Creates a new user and returns an authentication token
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
          [GenericRoute("acceptinvitation")]
        [HttpPost]
        [AllowAnonymous]
        public IHttpActionResult AcceptInvitation(NewUser user)
        {
            try
            {
                if (user == null)
                {
                    throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.PreconditionFailed));
                }

                AuthenticationToken token = authService.CreateUser(user);
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
         /// Using the reset guid sent in the email, accepts a new password and completes the reset process
         /// </summary>
         /// <param name="completionObject"></param>
         /// <returns>AuthenticationToken</returns>
          [GenericRoute("reset/complete")]
          [HttpPost]
          [AllowAnonymous]
          public IHttpActionResult CompleteResetPassword(PasswordResetCompletion completionObject)
          {
              try
              {
                  AuthenticationToken token = authService.CompletePasswordReset(completionObject.ResetGuid, completionObject.NewPassword);
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
          public IHttpActionResult ChangePassword(PasswordChange change)
          {
              try
              {

                  AuthenticationToken token = authService.ChangePassword(change.UserName, change.OldPassword, change.NewPassword);
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


          /// <summary>
          ///  Inititiates a PassReset that is completed via email
          /// </summary>
          /// <param name="user"></param>
          /// <returns></returns>
          [GenericRoute("reset/initiate")]
          [HttpPost]
          [AllowAnonymous]
          public IHttpActionResult ResetPassword(PasswordReset resetObject)
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
          public IHttpActionResult ExtendTokenValidity([FromBody]string token)
          {
              var certificate = DataReef.TM.Api.Security.Certificates.Certificate.Get();
              var authToken = AuthenticationToken.FromEncryptedString(token, certificate);
              authToken.Expiration = DateTime.UtcNow.AddDays(7).ToUnixTime();
              return Ok<Jwt>(EncodeToken(authToken));
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
                  Expiration    = token.Expiration,
                  Token         = tokenString
              };
          }

          #endregion
    }
}
