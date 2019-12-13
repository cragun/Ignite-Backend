using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace DataReef.TM.Models.Spruce
{
    [Table("Properties", Schema = "Spruce")]
    public class SpruceProperty : EntityBase
    {
        [DataMember]
        public decimal MonthlyMortgagePayment { get; set; }

        [DataMember]
        public string TitleHolder { get; set; }

        #region Navigation

        [DataMember]
        [Required]
        public QuoteRequest QuoteRequest { get; set; }

        #endregion
    }
}
