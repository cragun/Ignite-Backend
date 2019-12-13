using System;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.ComponentModel;
using System.Collections.Generic;
using DataReef.Core.Attributes;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataReef.TM.Models.Reporting
{
    [Table("Reports", Schema = "reporting")]
    public class Report:EntityBase
    {
        
        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public string RowColor { get; set; }

        [DataMember]
        public string AlternateRowColor { get; set; }

        [JsonIgnore]
        [DataMember]
        public string SqlCommand { get; set; }

        [DataMember]
        [InverseProperty("Report")]
        public ICollection<Column> Columns { get; set; }

        [DataMember]
        [InverseProperty("Report")]
        public ICollection<Parameter> Parameters { get; set; }

        [DataMember]
        [InverseProperty("Report")]
        [AttachOnUpdate]
        public ICollection<OUReport> OUReports { get; set; }

    }
}
