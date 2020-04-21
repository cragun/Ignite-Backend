using DataReef.TM.Models.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace DataReef.TM.Models
{
    [Table("PersonClockTime")]
    public class PersonClockTime : EntityBase
    {

        [DataMember]
        public Guid PersonID { get; set; }

        [DataMember]
        public Guid CreatedByID { get; set; }

        [DataMember]
        public Guid LastModifiedBy { get; set; }

        [DataMember]
        public string ClockType { get; set; }

        [DataMember]
        public string CreatedByName { get; set; }

        [DataMember]
        public string LastModifiedByName { get; set; }

        [DataMember]
        public long ClockDiff { get; set; }

         [DataMember]
        public bool IsDeleted { get; set; }

        [DataMember]
        public long Version { get; set; }

        [DataType(DataType.DateTime)]
        [DataMember]
        public DateTime? StartDate { get; set; }

        [DataType(DataType.DateTime)]
        [DataMember]
        public DateTime? DateLastModified { get; set; }

        [DataType(DataType.DateTime)]
        [DataMember]
        public DateTime? DateCreated { get; set; }

        [DataType(DataType.DateTime)]
        [DataMember]
        public DateTime? EndDate { get; set; }

        #region Navigation

        [ForeignKey("PersonID")]
        [DataMember]
        public User user { get; set; }

        #endregion Navigation
    }
}
