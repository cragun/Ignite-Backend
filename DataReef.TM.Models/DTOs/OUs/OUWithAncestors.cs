using DataReef.TM.Models.DataViews;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models.DTOs.OUs
{
    [DataContract]
    [NotMapped]
    public class OUWithAncestors : GuidNamePair
    {
        [DataMember]
        public IEnumerable<GuidNamePair> Ancestors { get; set; }
    }
}
