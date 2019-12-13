using System;

namespace DataReef.TM.Models.DTOs.Reports
{
    public class AuthenticationSummary
    {
        public Guid UserID { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string RootOUNames { get; set; }
        public int AuthenticatedDeviceCount { get; set; }
        public DateTime? LastAuthenticatedDate { get; set; }
        public DateTime? DeletedDate { get; set; }
    }
}