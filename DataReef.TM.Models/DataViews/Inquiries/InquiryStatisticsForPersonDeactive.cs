using DataReef.TM.Models.Enums;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace DataReef.TM.Models.DataViews.Inquiries
{
    [DataContract]
    [NotMapped]
    public class InquiryStatisticsForPersonDeactive
    {
        [DataMember]
        [JsonIgnore]
        public Guid PersonId { get; set; }
        [DataMember]
        public string Name   { get; set; }
        [DataMember]
        public InquiryStatisticsByDateDeactive Actions { get; set; }
        [DataMember]
        public InquiryStatisticsByDateDeactive DaysActive { get; set; }
    }
}
