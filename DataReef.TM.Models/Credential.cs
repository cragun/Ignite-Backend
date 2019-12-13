using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;


namespace DataReef.TM.Models
{
    public class Credential:EntityBase
    {
        /// <summary>
        /// Guid of the User file
        /// </summary>
        [DataMember]
        public Guid UserID { get; set; }

        /// <summary>
        /// Guid of the Person.  A bit denormalized.  No foreign key or navigation.  Navigation done thorugh UserID
        /// </summary>
        [DataMember]
        public Guid PersonID { get; set; }

        /// <summary>
        /// The username used to log in (email address)
        /// </summary>
        [DataMember]
        [StringLength(50)]
        public string UserName { get; set; }

        /// <summary>
        /// The hash
        /// </summary>
        [DataMember]
        [JsonIgnore]
        [StringLength(100)]
        public string PasswordHashed { get; set; }

        /// <summary>
        /// Used to reset the password.  Sent in by user in the rawformat, but never stored, rather salted and hashed, then cleared
        /// </summary>
        [DataMember]
        [NotMapped]
        public string PasswordRaw { get; set; }

        /// <summary>
        /// Salt used to calculate hash
        /// </summary>
        [DataMember]
        [JsonIgnore]
        public string Salt { get; set; }

        /// <summary>
        /// date the password expires.  null for never
        /// </summary>
        [DataMember]
        public System.DateTime? ExpirationDate { get; set; }

        /// <summary>
        /// Is the user required to change password on next loginn
        /// </summary>
        [DataMember]
        public bool RequiresPasswordChange { get; set; }


        #region Navigation

        [ForeignKey("UserID")]
        [DataMember]
        public User User { get; set; }

        #endregion

        public override void FilterCollections<T>(string inclusionPath = "")
        {
            bool alreadyProcessed;
            string newInclusionPath = InclusionPathBuilder(inclusionPath, GetType().Name, out alreadyProcessed);
            if (alreadyProcessed)
            {
                return;
            }

            User = FilterEntity(User, newInclusionPath);

        }


        public void PerformHash()
        {
           
            this.Salt = DataReef.Auth.Helpers.CryptographyHelper.GenerateSalt();
            this.PasswordHashed = DataReef.Auth.Helpers.CryptographyHelper.ComputePasswordHash(this.PasswordRaw, this.Salt);
            this.PasswordRaw = null;

        }


    }
}
