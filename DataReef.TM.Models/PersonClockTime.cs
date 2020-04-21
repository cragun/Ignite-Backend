﻿using DataReef.TM.Models.Enums;
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
        public string ClockType { get; set; }

        [DataMember]
        public long ClockDiff { get; set; }

        [DataType(DataType.DateTime)]
        [DataMember]
        public DateTime? StartDate { get; set; }

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