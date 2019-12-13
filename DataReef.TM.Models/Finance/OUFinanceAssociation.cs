using DataReef.Core.Attributes;
using DataReef.TM.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace DataReef.TM.Models.Finance
{
    [Table("OUAssociation", Schema = "finance")]
    public class OUFinanceAssociation: EntityBase
    {
        [DataMember]
        public Guid OUID { get; set; }

        [DataMember]
        public Guid FinancePlanDefinitionID { get; set; }
      
        #region Navigation

        [DataMember]
        [ForeignKey("OUID")]
        public OU OU { get; set; }


        [DataMember]
        [ForeignKey("FinancePlanDefinitionID")]
        public FinancePlanDefinition FinancePlanDefinition { get; set; }




        #endregion
    }
}