using DataReef.TM.Contracts.Services;
using Microsoft.Owin;
using Microsoft.Owin.Security.Infrastructure;
using Microsoft.Practices.ServiceLocation;

namespace DataReef.TM.ClientApi.Middlewares.Auth
{
    public class TokenAuthMiddleware : AuthenticationMiddleware<TokenAuthOptions>
    {
        public TokenAuthMiddleware(OwinMiddleware nextMiddleware, TokenAuthOptions authOptions)
            : base(nextMiddleware, authOptions)
        {
        }

        protected override AuthenticationHandler<TokenAuthOptions> CreateHandler()
        {
            var serviceLocator = ServiceLocator.Current.GetInstance<IClientAuthService>();

            return new TokenAuthHandler(serviceLocator, Options.AuthenticationType);
        }
    }
}