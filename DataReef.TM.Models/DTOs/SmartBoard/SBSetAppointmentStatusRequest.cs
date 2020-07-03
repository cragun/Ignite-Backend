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
    public class SBCreateAppointmentRequest
    {
        [DataMember]
        public long LeadID { get; set; }

        [DataMember]
        public string CreatedByID { get; set; }

        [DataMember]
        public string AssignedToID { get; set; }

        [DataMember]
        public string AppointmentDescription { get; set; }

        [DataMember]
        public string EmailsToCC { get; set; }

        [DataMember]
        public DateTime StartDate { get; set; }

        [DataMember]
        public DateTime? EndDate { get; set; }

        [DataMember]
        public string GoogleEventID { get; set; }

        [DataMember]
        public string TimeZone { get; set; }

        [DataMember]
        public int AppointmentType { get; set; }
    }
}
