using DataReef.Core.Attributes;
using DataReef.TM.Models.Enums;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace DataReef.TM.Models.Finance
{
    [Table("Providers", Schema = "finance")]
    public class FinanceProvider : EntityBase
    {
        [DataMember]
        public string ImageUrl { get; set; }

        [DataMember]
        public string PhoneNumber { get; set; }

        /// <summary>
        /// This property will be used to store the UX flow used when the user would build a proposal.
        /// </summary>
        [DataMember]
        public FinanceProviderProposalFlowType ProposalFlowType { get; set; }

        #region Navigation

        [DataMember]
        [InverseProperty("Provider")]
        [AttachOnUpdate]
        public ICollection<FinancePlanDefinition> FinancePlanDefinitions { get; set; }

        #endregion
    }
}