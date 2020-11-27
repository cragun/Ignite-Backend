using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataReef.TM.Models.DTOs.Persons
{
    public class PersonOffboard
    {
        public Guid OUID { get; set; }
        public Guid RoleID { get; set; }
        public string AssociateOuName { get; set; }
        public string RoleName { get; set; }
        public string Apikey { get; set; }
    }
}