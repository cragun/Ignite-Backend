using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace DataReef.TM.Models.DataViews.Inquiries
{
    [DataContract]
    [NotMapped]
    public class InquiryStatisticsByDateDeactive
    {
        [DataMember(Name= "AllTime", EmitDefaultValue=false)]
        public int AllTime { get; set; }

        [DataMember(Name= "ThisYear", EmitDefaultValue=false)]
        public int ThisYear { get; set; }

        [DataMember(Name= "ThisMonth", EmitDefaultValue=false)]
        public int ThisMonth { get; set; }

        [DataMember(Name= "ThisWeek", EmitDefaultValue=false)]
        public int ThisWeek { get; set; }

        [DataMember(Name= "Today", EmitDefaultValue=false)]
        public int Today { get; set; }

        [DataMember(Name= "SpecifiedDay", EmitDefaultValue=false)]
        public int SpecifiedDay { get; set; }

        [DataMember(Name= "ThisQuarter", EmitDefaultValue=false)]
        public int ThisQuarter { get; set; }

        [DataMember(Name= "RangeDay", EmitDefaultValue=false)]
        public int RangeDay { get; set; }
    }
}
