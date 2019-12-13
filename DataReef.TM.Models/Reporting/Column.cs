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
     [Table("Columns", Schema = "reporting")]
    public class Column : EntityBase
    {
        [DataMember]
        public Guid ReportID { get; set; }

        [DataMember]
        public int Width { get; set; }

        [DataMember]
        public int Position { get; set; }

        [DataMember]
        public string FormatString { get; set; }

        [DataMember]
        public string Caption { get; set; }

        [DataMember]
        public bool IsHidden { get; set; }


        /// <summary>
        /// if this has a guid of a report, the ui will render a link
        /// </summary>
        [DataMember]
        public Guid? DrillDownReportID { get; set; }

        [DataMember]
        public string DrillDownParameterName { get; set; }


        [ForeignKey("ReportID")]
        public Report Report { get; set; }



    }
}
