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
    [Table("OUReports", Schema = "reporting")]
    public class OUReport : EntityBase
    {
        [DataMember]
        public Guid ReportID { get; set; }

        [DataMember]
        public Guid OUID { get; set; }


        [ForeignKey("ReportID")]
        [DataMember]
        public Report Report { get; set; }

        [ForeignKey("OUID")]
        [DataMember]
        public OU OU { get; set; }


    }
}
