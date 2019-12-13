using System;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.ComponentModel;
using System.Collections.Generic;
using DataReef.Core.Attributes;
using Newtonsoft.Json;

namespace DataReef.TM.Models.Reporting
{
    [Table("Parameters", Schema = "reporting")]
    public class Parameter : EntityBase
    {
        [DataMember]
        public Guid ReportID { get; set; }

         /// <summary>
         /// OU,Date,Int,
         /// </summary>
        [DataMember]
        public string ParameterType { get; set; }

        [DataMember]
        public bool IsRequired { get; set; }

        [DataMember]
        public bool IsStartDate { get; set; }

        [DataMember]
        public bool IsEndDate { get; set; }

        [DataMember]
        public bool IsOUID { get; set; }

        [ForeignKey("ReportID")]
        public Report Report { get; set; }



    }
}
