using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models.DTOs.Persons
{
    public class CreateUserDTO
    {
        public string EmailAddress { get; set; }

        public string Password { get; set; }

        public string PhoneNumber { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public Guid RoleID { get; set; }

        public int ID { get; set; }

        public string[] apikey { get; set; }
    }
}
