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
    public class WebHook : EntityBase
    {
        /// <summary>
        /// An user is and abstraction of a person that can login into the system (Employee or Parent)
        /// </summary>
        [DataMember]
        public Guid OUID { get; set; }

        [DataMember]
        public bool IsDisabled { get; set; }

        /// <summary>
        /// URL to call, includes http:// or https://
        /// </summary>
        [DataMember]
        [StringLength(1000)]
        public string Url { get; set; }

        /// <summary>
        /// Name of the queue where we should also store the WebHookCall
        /// </summary>
        [DataMember]
        [StringLength(1000)]
        public string QueueName { get; set; }



        /// <summary>
        /// Which events to we want to send to this URL.  0(All) is default
        /// </summary>
        [DataMember]
        public EventDomain EventFlags { get; set; }

        /// <summary>
        /// Optional string that the receiver expects.  If provided, we simply pass on this value so the receiver (client) can validate if it comes from us
        /// </summary>
        public string PrivateKey { get; set; }

        /// <summary>
        /// Email that we use to send error or other critical notifications regarding webhook callbacks
        /// </summary>
        [DataMember]
        [StringLength(150)]
        public string NotificationEmailAddress { get; set; }


        #region Navigation

        /// <summary>
        /// Navigation property for the Person
        /// </summary>
        [DataMember]
        [Required]
        [ForeignKey("OUID")]
        public OU OU { get; set; }


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

            
        }
    }
}
