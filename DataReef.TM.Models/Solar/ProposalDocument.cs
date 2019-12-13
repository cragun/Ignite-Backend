using DataReef.TM.Models.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace DataReef.TM.Models.Solar
{
    [Table("FinanceDocuments", Schema = "solar")]
    public class FinanceDocument : EntityBase
    {
        /// <summary>
        /// We add an index on this column to quickly find it when RightSignature callsback
        /// </summary>
        [Index("IDX_DocumenID")]
        [DataMember]
        [MaxLength(250)]
        public string DocumentID { get; set; }

        [DataMember]
        public FinanceDocumentType DocumentType { get; set; }

        [DataMember]
        public string SignedURL { get; set; }

        [DataMember]
        public string UnsignedURL { get; set; }

        [DataMember]
        public DateTime? SignedDate { get; set; }

        [DataMember]
        public DateTime? ExpiryDate { get; set; }

        [DataMember]
        public Guid FinancePlanID { get; set; }

        [DataMember]
        public string ErrorMessage { get; set; }

        #region Navigation

        [DataMember]
        [ForeignKey("FinancePlanID")]
        public FinancePlan FinancePlan { get; set; }

        #endregion

        public FinanceDocument Clone(Guid financePlanID)
        {
            FinanceDocument ret = (FinanceDocument)this.MemberwiseClone();
            ret.Reset();
            ret.FinancePlanID = financePlanID;
            ret.FinancePlan = null;
            return ret;
        }
    }
}
