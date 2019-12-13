using DataReef.TM.Models.DataViews;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DataReef.TM.Api.Classes.ViewModels
{
    public class RegistrationViewModel
    {
        public Guid Guid { get; set; }
        public string Email { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Not a valid Phone number. Must be xxx-xxx-xxxx.")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [StringLength(255, ErrorMessage = "Password must have at least 6 characters", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public HttpPostedFileBase Photo { get; set; }

        public bool ShowFormIfModelHasErrors { get; set; }

        public NewUser ToNewUser()
        {
            return new NewUser
            {
                InvitationGuid = Guid,
                FirstName = FirstName,
                LastName = LastName,
                Password = Password
            };
        }
    }
}