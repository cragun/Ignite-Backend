using DataReef.TM.Models.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace DataReef.TM.Models.Credit
{
    /// <summary>
    /// Stores instant prescreen request and response data
    /// </summary>
    public class PrescreenInstant:EntityBase
    {
        [DataMember]
        public PrescreenSource Source { get; set; }

        /// <summary>
        /// All batches will begin with a pending process, then will move into Processing once the qualification server is contacted
        /// </summary>
        [DataMember]
        public PrescreenStatus Status { get; set; }
     
        /// <summary>
        /// The datetime that the process completed
        /// </summary>
        [DataMember]
        public DateTime? CompletionDate { get; set; }

        /// <summary>
        /// If the processed errored out ... the service should populate this error string
        /// </summary>
        [DataMember]
        public string ErrorString { get; set; }

        /// <summary>
        /// The guid of the property to be prescreened
        /// </summary>
        [DataMember]
        public Guid PropertyID { get; set; }

        /// <summary>
        /// use this for any meta data, like the name of the person who passed, or other human readable data we may wish to return
        /// </summary>
        [DataMember]
        [StringLength(150)]
        public string Reference { get; set; }

        /// <summary>
        /// The best credit category available for this specific AddressID
        /// </summary>
        [DataMember]
        public string CreditCategory { get; set; }

        #region Navigation

        [ForeignKey("PropertyID")]
        [DataMember]
        public Property Property { get; set; }

        #endregion
    }
}
