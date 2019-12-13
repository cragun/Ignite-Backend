using System;

namespace DataReef.TM.Classes
{
    public class UserInvitationTemplate : BaseTemplate
    {
        public string OUName { get; set; }
        public string InvitationURL { get; set; }
        public string InviterName { get; set; }
        public Guid? ToPersonId { get; set; }
        public string DownloadURL { get; set; }
        public bool UserWasDeleted { get; set; }        
    }
}
