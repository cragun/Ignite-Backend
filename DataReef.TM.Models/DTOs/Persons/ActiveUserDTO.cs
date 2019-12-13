using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;

namespace DataReef.TM.Models.DTOs.Persons
{
    public class ActiveUserDTO
    {
        private static List<string> RolesList = new List<string> { "Owner", "Manager", "Franchise Manager", "Photos Admin", "Photos Manager", "Installer", "Member" };

        [Ignore]
        public Guid Guid { get; set; }

        [Index(0)]
        public string FirstName { get; set; }

        [Index(1)]
        public string LastName { get; set; }

        [Index(2)]
        public string FullName { get; set; }

        [Index(3)]
        public string EmailAddressString { get; set; }

        [Index(4)]
        public string RoleName { get; set; }

        [Ignore]
        public int RoleIndex => RolesList.IndexOf(RoleName);
    }
}
