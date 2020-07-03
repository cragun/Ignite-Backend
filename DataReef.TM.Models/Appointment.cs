using DataReef.TM.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models
{
    [Table("Appointments")]
    public class Appointment : EntityBase
    {
        [DataMember]
        public Guid PropertyID { get; set; }

        [DataMember]
        public Guid? AssigneeID { get; set; }

        [DataMember]
        public DateTime StartDate { get; set; }

        [DataMember]
        public DateTime? EndDate { get; set; }

        [DataMember]
        public double? Latitude { get; set; }

        [DataMember]
        public double? Longitude { get; set; }

        [DataMember]
        [StringLength(250)]
        public string Address { get; set; }

        [DataMember]
        public string Details { get; set; }

        [DataMember]
        [StringLength(250)]
        public string GoogleEventID { get; set; }

        [DataMember]
        public AppointmentStatus Status { get; set; }

        [DataMember]
        public int AppointmentType { get; set; }

        [DataMember]
        [StringLength(250)]
        public string TimeZone { get; set; }

        #region Navigation Properties

        [DataMember]
        [ForeignKey(nameof(PropertyID))]
        public Property Property { get; set; }

        [DataMember]
        [ForeignKey(nameof(AssigneeID))]
        public Person Assignee { get; set; }

        [DataMember]
        [ForeignKey(nameof(CreatedByID))]
        public Person Creator { get; set; }

        #endregion
    }
}
