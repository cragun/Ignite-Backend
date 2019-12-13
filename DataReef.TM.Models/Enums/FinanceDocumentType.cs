using DataReef.Core.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models.Enums
{
    [DataContract]
    public enum FinanceDocumentType
    {
        [EnumMember]
        Unknown = 0,

        [TemplateName("ProposalTemplates")]
        [EnumMember]
        Proposal = 1,

        [TemplateName("AgreementTemplates")]
        [EnumMember]
        Agreement = 2,

        [TemplateName("ContractTemplates")]
        [EnumMember]
        Contract = 3
    }

}
