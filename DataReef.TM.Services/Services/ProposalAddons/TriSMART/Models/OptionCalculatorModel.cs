using DataReef.TM.Models.DTOs.Solar.Finance;
using DataReef.TM.Models.Finance;

namespace DataReef.TM.Services.Services.ProposalAddons.TriSMART.Models
{
    public class OptionCalculatorModel
    {
        public LoanRequest Request { get; set; }

        public int ScenarioTermInYears { get; set; }

        public string PlanName { get; set; }

        public FinancePlanDefinition PlanDefinition { get; set; }

        public double? UtilityInflationRate { get; set; }
    }
}
