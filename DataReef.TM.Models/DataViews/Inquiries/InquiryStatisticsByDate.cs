using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace DataReef.TM.Models.DataViews.Inquiries
{
    [DataContract]
    [NotMapped]
    public class InquiryStatisticsByDate
    {
        [DataMember]
        public int AllTime { get; set; }

        [DataMember]
        public int ThisYear { get; set; }

        [DataMember]
        public int ThisMonth { get; set; }

        [DataMember]
        public int ThisWeek { get; set; }

        [DataMember]
        public int Today { get; set; }

        [DataMember]
        public int SpecifiedDay { get; set; }
    }
}
