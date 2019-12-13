using DataReef.Core;
using DataReef.Core.Logging;
using DataReef.TM.Contracts.Auth;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace DataReef.TM.Api.Security
{
    public class TokenValidationHandler : DelegatingHandler
    {
        private const string OriginRequestHeader = "Origin";
        private const string BasicAuthResponseHeader = "WWW-Authenticate";
        private const string BasicAuthResponseHeaderValue = "Basic";
        private readonly ILogger _logger;

        public TokenValidationHandler(ILogger logger)
        {

            if (logger == null) throw new ArgumentNullException("logger");
            _logger = logger;
        }

        private static void SetUnAuthenticatedPrincipal()
        {
            var claimsIdentity = new ClaimsIdentity(authenticationType: null); // is not authenticated
            var principal = new ClaimsPrincipal(claimsIdentity);

            Thread.CurrentPrincipal = principal;

            if (HttpContext.Current != null)
                HttpContext.Current.User = principal;
        }


        private static bool TryRetrieveToken(HttpRequestMessage request, out string token)
        {
            token = null;
            IEnumerable<string> authHeaders;
            if (!request.Headers.TryGetValues("Authorization", out authHeaders) || authHeaders.Count() > 1)
            {
                return false;
            }

            string authToken = authHeaders.First();
            
            if (authToken.ToLower() == "null") return false;

            if (authToken.StartsWith("Bearer "))
            {
                authToken = authToken.Replace("Bearer ", string.Empty);
            }

            if (authToken.StartsWith("Basic "))
            {
                authToken = authToken.Replace("Basic ", string.Empty);
                try
                {
                    // In Basic authentication, the header is username:password, Base64 encoded
                    authToken = Encoding.ASCII.GetString(Convert.FromBase64String(authToken));
                }
                catch { }

                authToken = authToken.Split(':')[0];
            }

            token = authToken;

            return true;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            HttpStatusCode statusCode;
            string token = string.Empty;

            if (!TryRetrieveToken(request, out token))
            {
                SetUnAuthenticatedPrincipal();
                return base.SendAsync(request, cancellationToken);
            }


            if (request.Method == HttpMethod.Options && request.Headers.Contains("Origin"))
            {
                string requestOrigin = request.Headers.GetValues(OriginRequestHeader).FirstOrDefault();
                IEnumerable<string> allowedOrigins = (ConfigurationManager.AppSettings["AllowedRequestOrigins"] ?? "").Split(';');

                if (allowedOrigins.Any(o => requestOrigin.StartsWith(o, StringComparison.CurrentCultureIgnoreCase)))
                {
                    HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
                    response.Headers.Add("Access-Control-Allow-Headers", "authorization");
                    response.Headers.Add("Access-Control-Allow-Origin", requestOrigin);

                    return Task<HttpResponseMessage>.Factory.StartNew(() => response);
                }
            }

            if(string.IsNullOrWhiteSpace(token))
            {
                statusCode = HttpStatusCode.Unauthorized;
                HttpResponseMessage response = new HttpResponseMessage(statusCode);
                response.Headers.Add(BasicAuthResponseHeader, BasicAuthResponseHeaderValue);

                return Task<HttpResponseMessage>.Factory.StartNew(() => response);
            }

            try
            {
                string issuerUri = ConfigurationManager.AppSettings["TokenIssuerUri"];

                try
                {
                    var certificate = DataReef.TM.Api.Security.Certificates.Certificate.Get();
                    var authToken = AuthenticationToken.FromEncryptedString(token, certificate);

                    if (authToken.Expiration < System.DateTime.UtcNow.ToUnixTime()) throw new SecurityTokenValidationException(@"This token has expired, please authenticate again.");

                    var claims = new List<Claim>();
                    claims.Add(new Claim(DataReefClaimTypes.AccountID, authToken.AccountID.ToString()));
                    claims.Add(new Claim(DataReefClaimTypes.TenantId, "1"));
                    claims.Add(new Claim(DataReefClaimTypes.UserId, authToken.UserID.ToString()));

                    ClaimsIdentity ci = new ClaimsIdentity(claims, "Custom");
                    ClaimsPrincipal principal = new ClaimsPrincipal(ci);

                    Thread.CurrentPrincipal = principal;
                    if (HttpContext.Current != null)
                    {
                        HttpContext.Current.User = principal;
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error("JWT validation error", ex);
                    return Task<HttpResponseMessage>.Factory.StartNew(() => new HttpResponseMessage(HttpStatusCode.Unauthorized));
                }

                return base.SendAsync(request, cancellationToken);
            }
            catch (SecurityTokenValidationException ex)
            {
                _logger.Error("JWT validation error", ex);
                statusCode = HttpStatusCode.Unauthorized;
            }
            catch (Exception ex)
            {
                _logger.Error("Intrernal server error", ex);
                statusCode = HttpStatusCode.InternalServerError;
            }

            return Task<HttpResponseMessage>.Factory.StartNew(() => new HttpResponseMessage(statusCode));
        }
    }
}
