using System;

namespace DataReef.Core.Infrastructure.Authorization
{
    public class AuthorizationContext
    {
        public Guid UserId { get; private set; }

        public string DeviceUuid { get; private set; }

        //public List<Role> Roles { get; set; }

        public int TenantId { get; private set; }

    }
}
