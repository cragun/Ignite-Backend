using Microsoft.Owin.Security;

namespace DataReef.TM.ClientApi.Middlewares.Auth
{
    public class TokenAuthOptions : AuthenticationOptions
    {
        public const string CustomTokenAuthentication = "CustomTokenAuthenticationType";

        public TokenAuthOptions() : base(CustomTokenAuthentication)
        {

        }
    }
}