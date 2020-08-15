using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models.DataViews.SelfTrackedKPIs
{
    [DataContract]
    [NotMapped]
    public class SelfTrackedStatistics
    {
        [DataMember]
        public string KPIName { get; set; }

        [DataMember]
        public SelfTrackedKPIByDate Actions { get; set; }
    }

    [DataContract]
    [NotMapped]
    public class InquiryStatistics
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public SelfTrackedKPIByDate Actions { get; set; }
    }
}
