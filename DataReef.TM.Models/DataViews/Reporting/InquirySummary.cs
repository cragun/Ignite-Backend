using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace DataReef.TM.Models.DataViews.Reporting
{
    [DataContract]
    [NotMapped]
    public class InquirySummaryItem
    {
        
        [DataMember]
        public string OUName { get; set; }

        [DataMember]
        public Guid OUID { get; set; }

        [DataMember]
        public Guid PersonID { get; set; }

        [DataMember]
        public string PersonName { get; set; }

        [DataMember]
        public int InquiryCount { get; set; }

        [DataMember]
        public int ContactCount { get; set; }

        [DataMember]
        public int SaleCount { get; set; }

        [DataMember]
        public Decimal SaleRate { get; set; }

        [DataMember]
        public Decimal ContactRate { get; set; }

        [DataMember]
        public Decimal CloseRate { get; set; }

    }
}