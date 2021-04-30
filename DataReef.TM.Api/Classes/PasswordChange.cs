using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

        [Required(ErrorMessage = "Password is required")]
        [NotEqual("OldPassword")]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z])(?=.*?[#?!@$%^&*-]).{8,50}$", ErrorMessage = "Password must have at least 8 characters and contain one uppercase letter, one lowercase letter, one digit and one special character.")]
        public string NewPassword { get; set; }
        public string fcm_token { get; set; }
    }
}