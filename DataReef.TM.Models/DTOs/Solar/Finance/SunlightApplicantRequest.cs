using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models.DTOs.Solar.Finance
{
   public class SunlightApplicantRequest
    {
        public IList<Projects> projects { get; set; }

        //public class Applicants
        //{
        //    public string firstName { get; set; }
        //    public string lastName { get; set; }
        //    public string email { get; set; }
        //    public string phone { get; set; }
        //    public bool isPrimary { get; set; }

        //}
        public class Projects
        {
            public string installStreet { get; set; }
            public string installCity { get; set; }
            public string installStateName { get; set; }
            public string installZipCode { get; set; }
            public IList<Applicants> applicants { get; set; }

        }
    }
}
