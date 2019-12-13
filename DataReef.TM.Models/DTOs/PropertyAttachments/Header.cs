using DataReef.TM.Models.Enums.PropertyAttachments;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models.DTOs.PropertyAttachments
{
    [DataContract]
    [NotMapped]
    public class Header
    {
        [DataMember]
        public string SystemTypeID { get; set; }

        [DataMember]
        public ItemStatus Status { get; set; }

        [DataMember]
        public DateTime CreatedDate { get; set; }

        [DataMember]
        public DateTime? UpdateDate { get; set; }

        [DataMember]
        public Customer Customer { get; set; }
    }
}
