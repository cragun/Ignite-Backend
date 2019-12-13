using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models.Surveys
{
    [Table("Survey72", Schema = "survey")]
    public class Survey72 : EntityBase
    {

        [Required]
        [DataMember]
        public string ClientName { get; set; }

        [DataMember]
        public string AlternateClientName { get; set; }

        [Required]
        [DataMember]
        public string StreetAddress { get; set; }

        [Required]
        [DataMember]
        public string City { get; set; }

        [Required]
        [DataMember]

        public string State { get; set; }

        [DataMember]
        public string ZipCode { get; set; }

        [Required]
        [DataMember]
        public string PhoneNumber { get; set; }

        [DataMember]
        public string AlternatePhoneNumber { get; set; }

        [DataMember]
        public string Email { get; set; }

        [DataMember]
        public string TypeOfResidence { get; set; }

        [DataMember]
        public int EquipmentAge { get; set; }

        [DataMember]
        public string EquipmentLocation { get; set; }

        [DataMember]
        public int AgeOfHome { get; set; }

        [DataMember]
        public string TypeOfAppointment { get; set; }

        [DataMember]
        public double Price { get; set; }

        [DataMember]
        public DateTime AppointmentDate { get; set; }

        [Required]
        [DataMember]
        public string AppointmentTimeInterval { get; set; }

        [DataMember]
        public string RepresentativeNotes { get; set; }

        [DataMember]
        public string RepresentativeName { get; set; }

        [DataMember]
        public string LeadSource { get; set; }

        #region Navigation properties

        [ForeignKey("Guid")]
        public Property Property { get; set; }

        #endregion
    }
}
