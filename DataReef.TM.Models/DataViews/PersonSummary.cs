using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models.DataViews
{
    [DataContract]
    [NotMapped]
    public class PersonSummary
    {

        [DataMember]
        public int ActiveTerritoryCount { get; set; }

        [DataMember]
        public int YTDSales { get; set; }

        [DataMember]
        public DateTime? LastInquiryDate { get; set; }

        [DataMember]
        public int SalesToday { get; set; }

        [DataMember]
        public int SalesThisWeek { get; set; }

        [DataMember]
        public int SalesThisMonth { get; set; }

        [DataMember]
        public int SalesThisYear { get; set; }

        [DataMember]
        public int SalesAllTime { get; set; }

    }
}
