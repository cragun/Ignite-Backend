using System.Collections.Generic;
using Thinktecture.IdentityServer.Core.Models;

namespace DataReef.Auth.IdentityServer.DataAccess
{
    internal static class StaticModels
    {
        internal static List<Client> Clients
        {
            get
            {
                return new List<Client>
                {
                    new Client
                    {
                        ClientName = "Resource Owner Flow Client",
                        Enabled = true,
                        ClientId = "mobile_client",
                        ClientSecret = "26vMe5bR",
                        Flow = Flows.ResourceOwner,
                        AccessTokenLifetime = 31104000,
                    
                        ScopeRestrictions = new List<string>
                        { 
                            "smart_api",
                        },
                                            
                        AccessTokenType = AccessTokenType.Jwt
                    }
                };
            }
        }

        internal static List<Scope> Scopes
        {
            get
            {
                return new List<Scope>
                {
                    new Scope
                    {
                        Name = "smart_api",
                        DisplayName = "SmartCare Web API",
                        Emphasize = false
                    }
                };
            }
        }
    }
}