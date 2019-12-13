using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace DataReef.TM.Models.Spruce
{
    [Table("QuoteResponses", Schema = "Spruce")]
    public class QuoteResponse: EntityBase
    {
        [DataMember]
        public long QuoteNumber { get; set; }

        [DataMember]
        public string Decision { get; set; }

        [DataMember]
        public string CreditResponse { get; set; }

        [DataMember]
        public DateTime? DecisionDateTime { get; set; }

        [DataMember]
        public decimal AmountFinanced { get; set; }

        [DataMember]
        public decimal LoanRate { get; set; }

        [DataMember]
        public int Term { get; set; }

        [DataMember]
        public decimal IntroRatePayment { get; set; }

        [DataMember]
        public int IntroTerm { get; set; }

        [DataMember]
        public decimal MonthlyPayment { get; set; }

        [DataMember]
        public int GotoTerm { get; set; }

        [DataMember]
        public string StipulationText { get; set; }

        [DataMember]
        public decimal MaxApproved { get; set; }

        #region Navigation

        [DataMember]
        [Required]
        public QuoteRequest QuoteRequest { get; set; }

        #endregion
    }
}
