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
    public class SelfTrackedKPIByDate
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
