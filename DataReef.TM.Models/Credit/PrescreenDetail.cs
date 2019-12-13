using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace DataReef.TM.Models.Credit
{
    /// <summary>
    /// This class stores the AddressID and the result for each item in a batch.  The actual address data is stored in the Geo server, but the AddressID included here is the reference
    /// </summary>
    public class PrescreenDetail:EntityBase
    {
        /// <summary>
        /// The guid of the batch that this belongs to
        /// </summary>
        [DataMember]
        public Guid BatchID { get; set; }

        /// <summary>
        /// The guid of the Address or Location in the Geo service
        /// </summary>
        [DataMember]
        public Guid? AddressID { get; set; }

        /// <summary>
        /// The guid of the Property in the TM server
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

        [ForeignKey("BatchID")]
        [DataMember]
        public PrescreenBatch Batch { get; set; }

        [ForeignKey("PropertyID")]
        [DataMember]
        public Property Property { get; set; }

        #endregion


    }
}
