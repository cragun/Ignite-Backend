using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace DataReef.TM.Models.DTOs.OUs
{
    [DataContract]
    [NotMapped]
    public class OrgIdSetting : OUSetting
    {
        [DataMember]
        public Guid OrgId { get; set; }

        public long Level { get; set; }
    }
}
