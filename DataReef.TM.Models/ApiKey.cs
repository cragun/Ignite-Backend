using DataReef.Core.Attributes;
using DataReef.TM.Models.Enums;
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
    [BootstrapExcluded(BootstrapType = BootstrapType.Api)]
    public class ApiKey : EntityBase
    {
        /// <summary>
        /// An user is and abstraction of a person that can login into the system (Employee or Parent)
        /// </summary>
        [DataMember]
        public Guid OUID { get; set; }

        [DataMember]
        public bool IsDisabled { get; set; }

        /// <summary>
        /// access key for API, string 20 characters
        /// </summary>
        [DataMember]
        [StringLength(20)]
        public string AccessKey { get; set; }


        /// <summary>
        /// hashed Secret Key
        /// </summary>
        [DataMember]
        [StringLength(50)]
        public string SecretKeyHash { get; set; }


        #region Navigation

        /// <summary>
        /// Navigation property for the Person
        /// </summary>
        [DataMember]
        [Required]
        [ForeignKey("OUID")]
        public OU OU { get; set; }

        [DataMember]
        [AttachOnUpdate]
        public ICollection<ApiToken> Tokens { get; set; }


        #endregion

        public override void FilterCollections<T>(string inclusionPath = "")
        {
            bool alreadyProcessed;
            string newInclusionPath = InclusionPathBuilder(inclusionPath, GetType().Name, out alreadyProcessed);
            if (alreadyProcessed)
            {
                return;
            }

            //  UserDevices = FilterEntityCollection(UserDevices, newInclusionPath);
            //  Credentials = FilterEntityCollection(Credentials, newInclusionPath);

            OU = FilterEntity(OU, newInclusionPath);
            Tokens = FilterEntityCollection(Tokens, newInclusionPath);



        }
    }
}
