using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;

namespace DataReef.TM.Models.DataViews
{
    /// <summary>
    /// The NewUser class is used to create a new user after verifying the users email address.  This is a data view ( special class ) and will
    /// only be used for creating the new user
    /// </summary>
    [NotMapped]
    public class NewUser
    {
        /// <summary>
        /// The guid returned in the registration reservation process
        /// </summary>
        /// 
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please include the Registration Guid")]
        [DataMember]
        public Guid InvitationGuid { get; set; }

        /// <summary>
        /// The raw password (pre salted and hashed) for the user.  The system will salt, hash and clear from memory this password
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please include the Users Password")]
        [DataMember]
        public string Password { get; set; }

        /// <summary>
        /// First name of the Person
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please include a First Name")]
        [DataMember]
        public string FirstName { get; set; }

        /// <summary>
        /// Last Name of the Person
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please include a Last Name")]
        [DataMember]
        public string LastName { get; set; }

       
    }
}
