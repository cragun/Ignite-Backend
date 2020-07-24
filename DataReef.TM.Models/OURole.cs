using DataReef.TM.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace DataReef.TM.Models
{
    public class OURole : EntityBase
    {
        public static readonly Guid SuperUserID = new Guid("FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFFFF");
        public static readonly Guid PaymentProcessorID = new Guid("ffffffff-1111-0000-0000-000000000000");
        public static readonly Guid OwnerRoleID = new Guid("21AE9075-2DEE-48A6-8A32-444580EB36F4");
        public static readonly Guid MemberRoleID = new Guid("afb4c680-1607-49ca-8220-879e43318075");
        public static readonly Guid AdministratorRoleID = new Guid("01134b47-abc9-4e8c-b6eb-e8b4579c2e87");
        public static readonly Guid UserRoleID = new Guid("564c397a-cf2e-4930-936d-afde1a5de547");

        public OURole()
        {
            IsActive = true;
            AddDefaultExcludedProperties("OUAssociations");
        }

        [DataMember(EmitDefaultValue = true)]
        public bool IsActive { get; set; }

        [DataMember]
        public bool IsAdmin { get; set; }

        [DataMember]
        public bool IsOwner { get; set; }

        [InverseProperty("OURole")]
        [DataMember]
        public ICollection<OUAssociation> OUAssociations { get; set; }

        [DataMember]
        public OURoleType RoleType { get; set; } = OURoleType.Member;

        [DataMember]
        public PermissionType Permissions { get; set; }

        public override void FilterCollections<T>(string inclusionPath = "")
        {
            bool alreadyProcessed;
            string newInclusionPath = InclusionPathBuilder(inclusionPath, GetType().Name, out alreadyProcessed);
            if (alreadyProcessed)
            {
                return;
            }

            OUAssociations = FilterEntityCollection(OUAssociations, newInclusionPath);

        }

        /// <summary>
        /// Increase version and update DateLastModified
        /// </summary>
        public void Updated(Guid? modifiedBy = null, string modifiedByName = null)
        {
            Version += 1;
            DateLastModified = DateTime.UtcNow;
            LastModifiedBy = modifiedBy;
            LastModifiedByName = modifiedByName;
        }


        public bool ShouldSerializeOUAssociations() { return false; }

        public bool IsGreaterThanOrEqual(OURole role)
        {
            if (role == null)
            {
                return true;
            }

            return IsOwner  // is the owner
                || (!IsOwner && IsAdmin && !role.IsOwner) // is the admin and the other role is not greater (Owner)
                || (!IsOwner && !IsAdmin && !role.IsOwner && !role.IsAdmin); // none of the roles are owner nor admin
        }
    }
}
