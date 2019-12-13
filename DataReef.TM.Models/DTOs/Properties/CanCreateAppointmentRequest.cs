using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models.DTOs.Properties
{
    [DataContract]
    [NotMapped]
    public class CanCreateAppointmentRequest
    {
        [DataMember]
        public Guid? PropertyID { get; set; }

        [DataMember]
        public string ExternalID { get; set; }

        [DataMember]
        public Guid? TerritoryID { get; set; }
    }
}
