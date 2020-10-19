using DataReef.TM.Models.DTOs.Inquiries;
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
    [Table("QuotasCommitments")]
    public class QuotasCommitment : EntityBase
    {
        [DataMember]
        public int Type { get; set; }

        [DataMember]
        public Guid RoleID { get; set; }

        [DataMember]
        public Guid PersonID { get; set; }

        [DataMember]
        public DateTime StartDate { get; set; }

        [DataMember]
        public DateTime EndDate { get; set; }

        [DataMember]
        public string dispositions { get; set; }

        [NotMapped]
        public List<CRMDisposition> Disposition { get; set; }

        [NotMapped]
        public string week { get; set; }

        [NotMapped]
        public int durations { get; set; }

        #region Navigation Properties

        [DataMember]
        [ForeignKey("RoleID")]
        public OURole OURole { get; set; }
         
        #endregion
         
        public override void FilterCollections<T>(string inclusionPath = "")
        {
            bool alreadyProcessed;
            string newInclusionPath = InclusionPathBuilder(inclusionPath, GetType().Name, out alreadyProcessed);
            if (alreadyProcessed)
            {
                return;
            }
             
            OURole = FilterEntity(OURole, newInclusionPath); 
        }
    }
}
