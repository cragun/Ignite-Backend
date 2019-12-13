using DataReef.TM.Models.Enums;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace DataReef.TM.Models
{
    public class AreaPurchase : EntityBase
    {
        #region Properties

        [DataMember]
        public AreaPurchaseStatus Status { get; set; }

        /// <summary>
        /// the guid of the person making the purchase
        /// </summary>
        [DataMember]
        public Guid PersonID { get; set; }

        /// <summary>
        /// the guid of the activated area
        /// </summary>
        [DataMember]
        public Guid AreaID { get; set; }

        /// <summary>
        /// The guid of the ou that owns the zip area
        /// </summary>
        [DataMember]
        public Guid OUID { get; set; }

        /// <summary>
        /// the date when the process completed
        /// </summary>
        [DataMember]
        public DateTime? CompletionDate { get; set; }

        [DataMember]
        public string ErrorString { get; set; }

        [DataMember]
        public int NumberOfTokens { get; set; }

        [DataMember]
        public float TokenPriceInDollars { get; set; }

        #endregion Properties

        #region Navigation

        [ForeignKey("PersonID")]
        public Person Person { get; set; }

        [ForeignKey("AreaID")]
        public ZipArea Area { get; set; }

        [DataMember]
        [ForeignKey("OUID")]
        public OU OU { get; set; }

        #endregion Navigation
    }
}
