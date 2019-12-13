using DataReef.Engines.FinancialEngine.Models;
using DataReef.Integrations.LoanPal;
using DataReef.Integrations.LoanPal.Models;
using DataReef.TM.Models.DTOs.Solar.Finance;
using DataReef.TM.Models.Finance;

namespace DataReef.Engines.FinancialEngine.Integrations
{
    public class LoanPalIntegrations
    {
        private readonly ILoanPalBridge _bridge;
        public LoanPalIntegrations(ILoanPalBridge bridge)
        {
            _bridge = bridge;
        }

        public LoanAmortizationData BuildLoanAmortizationData(LoanRequest request, FinancePlanDefinition planDefinition)
        {
            var data = _bridge.CalculateLoan(new LoanCalculatorRequest
            {
                Amount = request.AmountToFinance,
                APR = planDefinition.Apr,
                TermInYears = planDefinition.TermInYears,
                ITC = request.FederalTaxIncentivePercentage,
                IncludeTable = true,
            });

            return data.ToDRAmortization(request, planDefinition);
        }
    }
}
