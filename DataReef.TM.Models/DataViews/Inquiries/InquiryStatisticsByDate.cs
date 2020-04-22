using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace DataReef.TM.Models.DataViews.Inquiries
{
    [DataContract]
    [NotMapped]
    public class InquiryStatisticsByDate
    {
        [DataMember]
        public long AllTime { get; set; }

        [DataMember]
        public long ThisYear { get; set; }

        [DataMember]
        public long ThisMonth { get; set; }

        [DataMember]
        public long ThisWeek { get; set; }

        [DataMember]
        public long Today { get; set; }

        [DataMember]
        public long SpecifiedDay { get; set; }

        [DataMember]
        public long ThisQuarter { get; set; }

        [DataMember]
        public long RangeDay { get; set; }
    }
}
