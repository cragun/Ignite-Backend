using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models.DTOs.OUs
{
    public class OUTreePath
    {
        public string Name { get; set; }
        //public string Guid { get; set; }
        //public string ParentID { get; set; }
        public Guid Guid { get; set; }
        public Guid? ParentID { get; set; }
        public string TreePath { get; set; }

        public Guid GetGuid()
        {
            return Guid; //new Guid(Guid);
        }

        public Guid? GetParentID()
        {
            return ParentID; //string.IsNullOrEmpty(ParentID) ? (Guid?)null : new Guid(ParentID);
        }
    }
}
