using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using DataReef.TM.Contracts.Services;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Infrastructure;

namespace DataReef.TM.ClientApi.Middlewares.Auth
{
    public class TokenAuthHandler : AuthenticationHandler<TokenAuthOptions>
    {
        private readonly IClientAuthService clientAuthService;
        private readonly string authType;

        public TokenAuthHandler(IClientAuthService clientAuthService, string authType)
        {
            this.clientAuthService = clientAuthService;
            this.authType = authType;
        }

        protected override async Task<AuthenticationTicket> AuthenticateCoreAsync()
        {
            var props = new AuthenticationProperties();
            try
            {
                var token = Request.Headers.Get("X-DataReef-Token");

                if(token!=null)
                {
                    // now validate against database, todo must cache here!
                    var tokenClaims = await clientAuthService.ValidateToken(new Guid(token));

                    var claims = new List<Claim>();
                    foreach (var tokenClaim in tokenClaims)
                        claims.Add(new Claim(tokenClaim.Key, tokenClaim.Value));

                    var ret = new ClaimsIdentity(claims, authType);
                    return new AuthenticationTicket(ret, props);
                }

                return new AuthenticationTicket(null, null);
               
             
            }
            catch
            {
                return new AuthenticationTicket(null, null);
            }
        }
    }
}