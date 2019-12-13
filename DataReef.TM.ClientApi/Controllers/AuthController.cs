using System;
using System.Security.Cryptography;
using System.Web.Http;
using DataReef.TM.ClientApi.Models;
using DataReef.TM.Contracts.Services;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Web.Http.Description;

namespace DataReef.TM.ClientApi.Controllers
{
    /// <summary>
    /// Authorization
    /// </summary>
    [RoutePrefix("authenticate")]
    [AllowAnonymous]
    public class AuthController : ApiController
    {
        private readonly IClientAuthService authService;

        public AuthController(IClientAuthService authService)
        {
            this.authService = authService;
        }

        /// <summary>
        /// Get auth token
        /// </summary>
        /// <param name="apiKey">The issued apikey</param>
        /// <param name="timestamp">Timestamp</param>
        /// <param name="signature">The issued signature</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{apiKey}/{timestamp}/{signature}")]
        [ResponseType(typeof(AuthenticationResponse))]
        public IHttpActionResult GetAuthToken(string apiKey, long timestamp, string signature)
        {
            try
            {
                var token = authService.Authenticate(apiKey, timestamp, signature);
                var response = new AuthenticationResponse(token);

                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = new AuthenticationResponse(ex);
                /*// probably not a good idea to return 200
                 HttpContext.Current.Response.StatusCode = 400; // Return bad request?
                 or go for BadRequest(); if you don't need the client to parse the AuthenticationResponse but just relly on the error code
                 */
                return Ok(response);
            }
        }

        /// <summary>
        /// Validate token
        /// </summary>
        /// <param name="token">The token received from authenticate</param>
        /// <returns></returns>
        [HttpGet]
        [Route("validate/token/{token:guid}")]
        [ResponseType(typeof(string))]
        public HttpResponseMessage ValidateToken(Guid token)
        {
            var ret = authService.ValidateIntegrationToken(token);
            if (string.IsNullOrEmpty(ret))
            {
                return this.Request.CreateResponse(HttpStatusCode.Unauthorized);
            }
            var response = this.Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(ret, Encoding.UTF8, "application/json");
            return response;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet]
        [Route("help")]
        public string GetTimeStampAndSignature()
        {
            try
            {
                string accessKey = "HSfMJeFWOsUs8JO8SkHH";
                string secretKey = "cc(IjLqeMrOEvTYrc338wqhuk$tpT(*IQQanPm^8";

                TimeSpan span = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc));
                long ts = (long)Math.Floor(span.TotalSeconds);


                // Generate the hash
                System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
                HMACMD5 hmac = new HMACMD5(encoding.GetBytes(secretKey));
                byte[] hash = hmac.ComputeHash(encoding.GetBytes(accessKey + ts));

                // Convert hash to digital signature string
                string signature = BitConverter.ToString(hash).Replace("-", "").ToLower();

                string ret = string.Format("http://gateway.datareef.com/authenticate/{0}/{1}/{2}", accessKey, ts, signature);
                return ret;
            }
            catch (Exception ex)
            {

                throw;
            }

        }
    }
}