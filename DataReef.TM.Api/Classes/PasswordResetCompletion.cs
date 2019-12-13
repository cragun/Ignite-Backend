using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataReef.TM.Api.Classes
{
    public class PasswordResetCompletion
    {
        public Guid ResetGuid { get; set; }

        public string NewPassword { get; set; }
    }
}