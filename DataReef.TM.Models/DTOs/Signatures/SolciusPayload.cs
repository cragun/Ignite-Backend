using System.Configuration;
using DataReef.TM.Models.DTOs.Proposals;
using DataReef.TM.Models.Solar;

namespace DataReef.TM.Models.DTOs.Signatures
{
    public class SolciusPayload : ProposalUnformattedDataView
    {
        public SolciusPayload(Proposal proposal, FinancePlan financePlan) : base(new ProposalUnformattedDataViewConstructor { Proposal = proposal, FinancePlan = financePlan })
        {
            SharedKey = ConfigurationManager.AppSettings["Solcius.SharedKey"];
        }

        public string SharedKey { get; set; }
    }
}
