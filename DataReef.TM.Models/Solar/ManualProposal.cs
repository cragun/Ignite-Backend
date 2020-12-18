using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace DataReef.TM.Models.Solar
{
    [Table("ManualProposal", Schema = "solar")]
    public class ManualProposal : EntityBase
    {
        [DataMember]
        public double TotalBill { get; set; }

        [DataMember]
        public double TotalKWH { get; set; }

        [DataMember]
        public bool IsManual { get; set; }
    }
}
