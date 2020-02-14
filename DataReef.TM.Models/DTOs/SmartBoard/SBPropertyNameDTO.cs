using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models.DTOs.SmartBoard
{
    [DataContract]
    [NotMapped]
    public class SBPropertyNameDTO
    {
        [DataMember]
        public string ExistFirstName { get; set; }

        [DataMember]
        public string ExistLastName { get; set; }

        [DataMember]
        public string ExistEmailAddress { get; set; }

        [DataMember]
        public string NewFirstName { get; set; }

        [DataMember]
        public string NewLastName { get; set; }

        [DataMember]
        public string NewEmailAddress { get; set; }

        [DataMember]
        public int? DispositionTypeId { get; set; }
    }
}
