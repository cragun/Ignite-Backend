using System;

namespace DataReef.TM.DataAccess.Prototype
{
    public class AuthorizationContext
    {
        public ApplicationRole ApplicationRole { get; set; }

        public Guid PersonGuid { get; set; }


        public long TenantId { get; set; }


        

    }
}
 