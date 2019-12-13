using DataReef.TM.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataReef.TM.Models.DTOs.OUs
{
    public class OUsAndRoleTree
    {
        public Guid OUID { get; set; }
        public Guid OURoleID { get; set; }
        public OURoleType RoleType { get; set; }

        public List<OUsAndRoleTree> Children { get; set; }

        public OUsAndRoleTree() { }

        public OUsAndRoleTree(OUAssociation assoc, List<OUTreePath> tree)
        {
            OUID = assoc.OUID;
            OURoleID = assoc.OURoleID;
            RoleType = assoc.RoleType;

            if (tree?.Any() != true)
            {
                return;
            }

            foreach (var item in tree)
            {
                AddOU(item, assoc.OURoleID, assoc.RoleType);
            }
        }

        public bool IsOwnerOrAdmin()
        {
            return OURoleID.IsOwnerOrAdmin();
        }

        public List<OUsAndRoleTree> GetOUAndChildren()
        {
            var result = Children?.SelectMany(c => c.GetOUAndChildren())?.ToList() ?? new List<OUsAndRoleTree>();
            result.Add(this);
            return result;
        }

        public bool HasChildOU(Guid ouid)
        {
            return OUID == ouid || Children?.Any(c => c.HasChildOU(ouid)) == true;
        }

        public void UpdateRole(Guid? ouid, Guid newRoleId, OURoleType roleType)
        {
            RoleType = RoleType | roleType;

            if (OURoleID.IsGreaterThan(newRoleId))
            {
                return;
            }

            if (!ouid.HasValue || OUID == ouid)
            {
                OURoleID = newRoleId;
                Children?.ForEach(c => c.UpdateRole(null, newRoleId, roleType));
            }
            else
            {
                Children?.ForEach(c => UpdateRole(ouid, newRoleId, roleType));
            }
        }

        public void AddOU(OUTreePath ou, Guid ouRoleId, OURoleType roleType)
        {
            if (OUID == ou.GetParentID())
            {
                Children = Children ?? new List<OUsAndRoleTree>();
                Children.Add(new OUsAndRoleTree { OUID = ou.GetGuid(), OURoleID = ouRoleId, RoleType = roleType });
            }
            else
            {
                Children?.ForEach(c => c.AddOU(ou, ouRoleId, roleType));
            }
        }
    }
}
