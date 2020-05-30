using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models.DTOs.Solar.Finance
{
    public class Applicants
    {
        public string id { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string phone { get; set; }
        public string email { get; set; }
        public bool isPrimary { get; set; }
        public string mailingStreet { get; set; }
        public string mailingCity { get; set; }
        public string mailingStateName { get; set; }
        public string mailingZipCode { get; set; }
        public string residenceStreet { get; set; }
        public string residenceCity { get; set; }
        public string residenceStateName { get; set; }
        public string residenceZipCode { get; set; }
        public bool? isCreditRun { get; set; }
    }
}
