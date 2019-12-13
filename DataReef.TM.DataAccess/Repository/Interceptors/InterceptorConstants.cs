using DataReef.Core.Helpers;
using DataReef.TM.Models;

namespace DataReef.TM.DataAccess.Repository.Interceptors
{
    public static class InterceptorConstants
    {
        public static readonly string DateCreatedProperty = TypeHelpers.GetPropertyName<EntityBase>(e => e.DateCreated);
        public static readonly string DateLastModifiedProperty = TypeHelpers.GetPropertyName<EntityBase>(e => e.DateLastModified);
        public static readonly string CreatedByProperty = TypeHelpers.GetPropertyName<EntityBase>(e => e.CreatedByID);
        public static readonly string LastModifiedByProperty = TypeHelpers.GetPropertyName<EntityBase>(e => e.LastModifiedBy);
        public static readonly string VersionProperty = TypeHelpers.GetPropertyName<EntityBase>(e => e.Version);
        public static readonly string TenantProperty = TypeHelpers.GetPropertyName<EntityBase>(e => e.TenantID);
    }
}
