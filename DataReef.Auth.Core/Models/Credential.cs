using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataReef.Auth.Core.Models
{
    public class Credential
    {
        public long Id { get; set; }

        public Credential()
        {
        }

        [StringLength(50)]
        public string Username { get; set; }

        [StringLength(50)]
        public string Password { get; set; }

        [StringLength(50)]
        public string Salt { get; set; }

        public bool RequirePasswordChange { get; set; }
      
        public virtual IList<User> Users { get; set; }

    
    }
}