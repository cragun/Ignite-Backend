using DataReef.Core.Attributes;
using DataReef.TM.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace DataReef.TM.Models
{
    public class ZipArea : EntityBase
    {
        #region Properties

        [DataMember]
        public ZIPAreaStatus Status { get; set; }

        [DataMember]
        public Guid OUID { get; set; }

        [DataMember]
        public int PropertyCount { get; set; }

        /// <summary>
        /// The date from where the data becomes available
        /// </summary>
        [DataMember]
        public DateTime? ActiveStartDate { get; set; }

        /// <summary>
        /// computed date of the last time where active status was set = date of most recent AreaPurchase.CompletionDate where AreaPurchase.Status = Pending OR AreaPurchase.Status = Completed
        /// </summary>
        [DataMember]
        public DateTime? LastPurchaseDate { get; set; }

        /// <summary>
        /// expressed in minutes, default value: 86400 mins = 60 days; Status = Active, ActiveStartDate + ExpiryMinutes < Current Date <=> Status = Expired (no need to be changed on the server side, it can be checked on the client)
        /// </summary>
        [DataMember]
        public int ExpiryMinutes { get; set; }

        #endregion Properties

        #region Navigation

        [ForeignKey("Guid")]
        public Zip5Shape Shape { get; set; }

        /// <summary>
        /// The ou that owns the zip area
        /// </summary>
        [DataMember]
        [ForeignKey("OUID")]
        public OU OU { get; set; }

        /// <summary>
        /// All the purchases for this zip area
        /// </summary>
        [DataMember]
        [InverseProperty("Area")]
        [AttachOnUpdate]
        public ICollection<AreaPurchase> Purchases { get; set; }

        #endregion Navigation
    }
}
