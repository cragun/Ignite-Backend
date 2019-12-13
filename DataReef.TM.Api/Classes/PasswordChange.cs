using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataReef.TM.Api.Classes
{
    /// <summary>
    /// This class is to be used to change your password.  When a password expires you cannot authenticate.  But you can call (unauthenticated) a change password
    /// </summary>
    public class PasswordChange
    {
        public string UserName { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }
}