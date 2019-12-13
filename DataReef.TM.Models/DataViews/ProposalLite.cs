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
    public class ProposalLite:CustomerLite
    {
        public ProposalLite()
        {

        }

     
        [DataMember]
        public string NameOfOwner { get; set; }

        [DataMember]
        public int SystemSize { get; set; }

        [DataMember]
        public int PanelCount { get; set; }

    }
}
