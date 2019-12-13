using DataReef.TM.Models.Enums;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace DataReef.TM.Models.DataViews.Inquiries
{
    [DataContract]
    [NotMapped]
    public class InquiryStatisticsForPerson
    {
        [DataMember]
        [JsonIgnore]
        public Guid PersonId { get; set; }
        [DataMember]
        public string Name   { get; set; }
        [DataMember]
        public InquiryStatisticsByDate Actions { get; set; }
        [DataMember]
        public InquiryStatisticsByDate DaysActive { get; set; }
    }
}
