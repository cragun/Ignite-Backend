using DataReef.TM.Models.DTOs.Solar.Finance;
using DataReef.TM.Services.Services.ProposalAddons.TriSMART.Models;

namespace DataReef.TM.Services.Services.ProposalAddons.TriSMART
{
    public interface ITrismartProposalEnhancement
    {
        LoanResponse CalculateOption(OptionCalculatorModel args);
    }
}
