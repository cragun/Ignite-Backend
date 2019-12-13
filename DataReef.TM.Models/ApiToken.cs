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

    public class ApiToken : EntityBase
    {
        /// <summary>
        /// An user is and abstraction of a person that can login into the system (Employee or Parent)
        /// </summary>
        [DataMember]
        public bool IsDisabled { get; set; }

        /// <summary>
        /// UTC DateTime 
        /// </summary>
        [DataMember]
        public System.DateTime ExpirationDate { get; set; }


        /// <summary>
        /// Guid of the ApiKey object
        /// </summary>
        [DataMember]
        public Guid ApiKeyID { get; set; }

        #region Navigation

      
        [DataMember]
        [ForeignKey("ApiKeyID")]
        public ApiKey ApiKey { get; set; }

        #endregion

        public override void FilterCollections<T>(string inclusionPath = "")
        {
            bool alreadyProcessed;
            string newInclusionPath = InclusionPathBuilder(inclusionPath, GetType().Name, out alreadyProcessed);
            if (alreadyProcessed)
            {
                return;
            }

            ApiKey = FilterEntity(ApiKey, newInclusionPath);


        }
    }
}
