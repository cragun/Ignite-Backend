using DataReef.TM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataReef.TM.ClientApi.Models
{
    public class OrganizationRoleDataView:OrganizationDataView
    {

        public OrganizationRoleDataView(OU ou):base(ou)
        {
          
        }


        public string Role { get; set; }
    }
}