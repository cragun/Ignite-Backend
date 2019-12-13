using DataReef.TM.Models.Enums;
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
    public class SBSetAppointmentStatusRequest
    {
        [DataMember]
        public Guid AppointmentID { get; set; }

        [DataMember]
        public AppointmentStatus Status { get; set; }
    }
}
