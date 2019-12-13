using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace DataReef.TM.Models.Spruce
{
    [Table("IncomeDebts", Schema = "Spruce")]
    public class IncomeDebt : EntityBase
    {
        [DataMember]
        public decimal AnnualIncome { get; set; }

        [DataMember]
        public Guid? AppQuoteId { get; set; }

        [DataMember]
        public Guid? CoAppQuoteId { get; set; }

        [DataMember]
        [NotMapped]
        public Guid QuoteId { get { return AppQuoteId ?? CoAppQuoteId ?? Guid.Empty; } }

        #region Navigation

        [DataMember]
        [ForeignKey("AppQuoteId")]
        public QuoteRequest AppQuote { get; set; }

        [DataMember]
        [ForeignKey("CoAppQuoteId")]
        public QuoteRequest CoAppQuote { get; set; }

        #endregion
    }
}
