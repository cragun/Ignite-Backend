using DataReef.Auth.Helpers;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace DataReef.TM.Models
{
    /// <summary>
    /// The credentials used to authenticate users on external systems such as APIs of credit institutions
    /// </summary>
    [DataContract]
    public class ExternalCredential : EntityBase
    {
        private const string cryptoKey = "7TuJCEjbGQZzLaNtdkbXTkdd";//q7KYLwNhYzVccCjc8S0pyYHu4izm2squk7NOPN5Y";

        [DataMember]
        [Index("IdxUserOU", Order = 0, IsClustered = false, IsUnique = false)]
        public Guid UserID { get; set; }

        [DataMember]
        [Index("IdxUserOU", Order = 1, IsClustered = false, IsUnique = false)]
        [MaxLength(400)]
        public string RootOrganizationName { get; set; }

        [DataMember]
        [JsonIgnore]
        public string Username { get; set; }

        [DataMember]
        [JsonIgnore]
        public string EncryptedPassword { get; set; }

        [NotMapped]
        [JsonIgnore]
        public string Password
        { 
            get
            {
                return CryptographyHelper.Decrypt(EncryptedPassword, cryptoKey);
            }
            set
            {
                this.EncryptedPassword = CryptographyHelper.Encrypt(value, cryptoKey);
            }
        }

        #region Navigation

        /// <summary>
        /// Navigation property for the Person
        /// </summary>
        [DataMember]
        [ForeignKey("UserID")]
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
    }
}
