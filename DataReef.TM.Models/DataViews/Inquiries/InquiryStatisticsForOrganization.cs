using DataReef.TM.Models.Enums;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace DataReef.TM.Models.DataViews.Inquiries
{
    [DataContract]
    [NotMapped]
    public class InquiryStatisticsForOrganization
    {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public InquiryStatisticsByDate Actions { get; set; }
        [DataMember]
        public InquiryStatisticsByDate People { get; set; }
    }
}
