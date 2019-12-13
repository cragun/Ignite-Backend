using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace DataReef.TM.Models.Spruce
{
    [Table("Inits", Schema = "Spruce")]
    public class Init : EntityBase
    {
        [DataMember]
        public string ContractorId { get; set; }

        [DataMember]
        public string Product { get; set; }

        [DataMember]
        public string Partner { get; set; }

        [DataMember]
        public string Program { get; set; }

        [DataMember]
        public string ContractorQuoteId1 { get; set; }

        [DataMember]
        public string ContractorQuoteId2 { get; set; }

        [DataMember]
        public string RateType { get; set; }

        [DataMember]
        public string FinOptId { get; set; }

        [DataMember]
        public string TermInMonths { get; set; }

        [DataMember]
        public string CashSalesPrice { get; set; }

        [DataMember]
        public string DownPayment { get; set; }

        [DataMember]
        public string AmountFinanced { get; set; }

        [DataMember]
        public string PrefAgreementTypeId { get; set; }

        [DataMember]
        public string DeliveryMethodId { get; set; }

        #region Navigation

        [DataMember]
        [Required]
        public QuoteRequest QuoteRequest { get; set; }

        #endregion
    }
}
