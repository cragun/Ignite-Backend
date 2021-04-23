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
       // [StringLength(255, ErrorMessage = "Password must have at least 8 characters", MinimumLength = 8)]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z])(?=.*?[#?!@$%^&*-]).{8,50}$", ErrorMessage = "Password must must have at least 8 characters and contain one uppercase letter, one lowercase letter, one digit and one special character.")]
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