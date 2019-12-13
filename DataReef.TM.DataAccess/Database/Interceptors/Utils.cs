using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;

namespace DataReef.TM.DataAccess.Database.Interceptors
{
    public static class SoftDeleteHelper
    {
        public static bool IsSoftDeleteEntity(EdmType type)
        {           
            var hasMetadata = type.MetadataProperties
                            .Where(p => p.Name.EndsWith("customannotation:SoftDeleteEntity"))
                            .Any();
            if (!hasMetadata)
                return false;

            var entityType = type as System.Data.Entity.Core.Metadata.Edm.EntityType;
            if(entityType.DeclaredProperties.Any(property=>property.Name == "IsDeleted"))
            {
                return true;
            }

            return false;
        }
    }
}
