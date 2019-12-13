using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace DataReef.TM.Models
{
    /// <summary>
    /// The User is the authentication/authorization abstraction in the API/Core
    /// The User has an underlying Person
    /// </summary>
    [DataContract]
    public class User : EntityBase
    {
        /// <summary>
        /// An user is and abstraction of a person that can login into the system (Employee or Parent)
        /// </summary>
        [DataMember]
        public Guid PersonID { get; set; }

        [DataMember]
        public bool IsDisabled { get; set; }

        [DataMember]
        [StringLength(50)]
        public string PartnerID { get; set; }

        [DataMember]
        public bool IsActive { get; set; }

        [DataMember]
        [JsonIgnore]
        public bool IsNonBillable { get; set; }

        /// <summary>
        /// Maximum number of devices allowed for this user.
        /// Default value is 1.
        /// </summary>
        [DataMember]
        [JsonIgnore]
        public int NumberOfDevicesAllowed { get; set; }

        #region Navigation

        /// <summary>
        /// Navigation property for the Person
        /// </summary>
        [DataMember]
        [Required]
        public Person Person { get; set; }

        /// <summary>
        /// Inverse navigation property.
        /// An user can login via multiple devices
        /// </summary>
        [DataMember]
        [InverseProperty("User")]
        public ICollection<UserDevice> UserDevices { get; set; }

        [DataMember]
        public ICollection<Credential> Credentials { get; set; }

        [DataMember]
        public ICollection<ExternalCredential> ExternalCredentials { get; set; }

        #endregion

        public User()
        {
            IsActive = true;
        }

        public override void FilterCollections<T>(string inclusionPath = "")
        {
            bool alreadyProcessed;
            string newInclusionPath = InclusionPathBuilder(inclusionPath, GetType().Name, out alreadyProcessed);
            if (alreadyProcessed)
            {
                return;
            }

            UserDevices = FilterEntityCollection(UserDevices, newInclusionPath);
            Credentials = FilterEntityCollection(Credentials, newInclusionPath);

            Person = FilterEntity(Person, newInclusionPath);

        }
    }
}
