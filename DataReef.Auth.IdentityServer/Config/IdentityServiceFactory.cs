using Thinktecture.IdentityServer.Core.Configuration;
using Thinktecture.IdentityServer.Core.Services;
using Thinktecture.IdentityServer.Core.Services.InMemory;
using DataReef.Auth.IdentityServer.Services;
using DataReef.Auth.IdentityServer.DataAccess;

namespace DataReef.Auth.IdentityServer.Config
{
    class IdentityServiceFactory
    {
        public static IdentityServerServiceFactory Configure()
        {
            IdentityServerServiceFactory factory = new IdentityServerServiceFactory();

            // Users
            factory.Register<DataContext>(Registration.RegisterType<DataContext>(typeof(DataContext)));
            factory.ClaimsProvider = Registration.RegisterType<IClaimsProvider>(typeof(UserClaimsProvider));
            factory.UserService = Registration.RegisterType<IUserService>(typeof(UserAuthenticationService));
            
            // Clients
            InMemoryClientStore clientStore = new InMemoryClientStore(StaticModels.Clients);
            factory.ClientStore = Registration.RegisterFactory<IClientStore>(() => clientStore);

            // Scopes
            InMemoryScopeStore scopeStore = new InMemoryScopeStore(StaticModels.Scopes);
            factory.ScopeStore = Registration.RegisterFactory<IScopeStore>(() => scopeStore);

            return factory;
        }
    }
}