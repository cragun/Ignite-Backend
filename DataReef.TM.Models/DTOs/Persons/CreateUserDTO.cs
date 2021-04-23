using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models.DTOs.Persons
{
    public class CreateUserDTO
    {
        public string EmailAddress { get; set; }

        [Required(ErrorMessage = "Password is required")]
        // [StringLength(255, ErrorMessage = "Password must have at least 8 characters", MinimumLength = 8)]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z])(?=.*?[#?!@$%^&*-]).{8,50}$", ErrorMessage = "Password must must have at least 8 characters and contain one uppercase letter, one lowercase letter, one digit and one special character.")]
        public string Password { get; set; }

        public string PhoneNumber { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public Guid RoleID { get; set; }

        public string ID { get; set; }

        public string[] apikey { get; set; }
    }
}
