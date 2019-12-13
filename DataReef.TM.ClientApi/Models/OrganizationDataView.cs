using DataReef.TM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataReef.TM.ClientApi.Models
{
    public class OrganizationDataView
    {
        public OrganizationDataView(OU ou)
        {
            this.Id = ou.Guid;
            this.Name = ou.Name;
            this.ParentID = ou.ParentID;
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public Guid? ParentID { get; set; }
    }
}