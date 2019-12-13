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
    public class CanCreateAppointmentResponse
    {
        [DataMember]
        public CanCreateAppointmentStatus Status { get; set; }

        [DataMember]
        public string DisplayMessage { get; set; }

        [DataMember]
        public Guid? PropertyID { get; set; }

        [DataMember]
        public Guid? TerritoryID { get; set; }
    }

    [DataContract]
    public enum CanCreateAppointmentStatus
    {
        [EnumMember]
        CanCreate,
        [EnumMember]
        AlreadyAssignedToMe,
        [EnumMember]
        AlreadyAssigned,
        [EnumMember]
        Error
    }
}
