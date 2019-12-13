using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace DataReef.TM.Models.Geo
{
    public class Field : EntityBase
    {       
        #region Properties

        [StringLength(100)]
        [DataMember]
        [Required]
        public string DisplayName { get; set; }

        [DataMember]
        [Required]
        public string Value { get; set; }

        [DataMember]
        public Guid? OccupantId { get; set; }

        [DataMember]
        public Guid? PropertyId { get; set; }

        #endregion


        #region Navigation

        [ForeignKey("OccupantId")]
        public Occupant Occupant { get; set; }

        [ForeignKey("PropertyId")]
        public Property Property { get; set; }

        #endregion

    }
}
