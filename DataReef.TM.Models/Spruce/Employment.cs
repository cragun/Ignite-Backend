using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace DataReef.TM.Models.Spruce
{
    [Table("Employments", Schema = "Spruce")]
    public class Employment : EntityBase
    {
        [DataMember]
        public string EmploymentStatus { get; set; }

        [DataMember]
        public DateTime EmployedSince { get; set; }

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
