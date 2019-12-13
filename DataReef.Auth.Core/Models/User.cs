using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataReef.Auth.Core.Models
{
    /// <summary>
    /// Represents a user inside an organization
    /// </summary>
    public class User
    {

        public long Id { get; set; }

        public User()
        {
           
        }

        /// <summary>
        /// The user ID inside the organization
        /// </summary>
        public Guid UserId { get; set; }
        
        public bool IsDisabled { get; set; }

        public virtual IList<Credential> Credentials { get; set; }

     
    }
}