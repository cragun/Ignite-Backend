using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models.DataViews
{
    /// <summary>
    /// This is the class used to Pre Register the user.  The user enters his / her email address which will cause an email to be sent to that address
    /// the email contains the User Registration Guid which will be subsequently used to confirm the registration and create the user
    /// </summary>
    [NotMapped]
    public class NewUserRegistration
    {
        [DataMember]
        public string EmailAddress { get; set; }

        [DataMember]
        public string FirstName { get; set; }

        [DataMember]
        public string LastName { get; set; }

    }
}
