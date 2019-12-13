using DataReef.Engines.FinancialEngine.Loan;
using DataReef.TM.Contracts.Services;
using DataReef.TM.DataAccess.Database;
using DataReef.TM.Models.DTOs.Solar.Finance;
using DataReef.TM.Models.Enums;
using DataReef.TM.Models.Finance;
using DataReef.TM.Services.Services.ProposalAddons.TriSMART;
using DataReef.TM.Services.Services.ProposalAddons.TriSMART.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;

namespace DataReef.TM.Services.Services
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(AddressFilterMode = AddressFilterMode.Any)]
    public class FinancialService : IFinancialService
    {
        private readonly IFinancingCalculator _loanCalculator;
        private readonly Lazy<ITrismartProposalEnhancement> _trismartEnhancement;

        public FinancialService(IFinancingCalculator loanCalculator,
                                Lazy<ITrismartProposalEnhancement> trismartEnhancement)
        {
            _loanCalculator = loanCalculator;
            _trismartEnhancement = trismartEnhancement;
        }

        public LoanResponse CalculateLease(LoanRequest request)
        {
            throw new NotImplementedException();
        }

        public LoanResponse CalculateLoan(Guid financePlanId, LoanRequest request)
        {
            throw new NotImplementedException();
        }

        public List<FinanceEstimate> CompareFinancePlans(FinanceEstimateComparisonRequest request)
        {
            using (var dataContext = new DataContext())
            {
                var query = dataContext
                            .FinancePlaneDefinitions
                            .Include(fpd => fpd.Details)
                            .Where(fpd => request.FinancePlanGuids.Contains(fpd.Guid))
                            .ToList();

                List<FinancePlanDefinition> plans = null;
                if (request.SortAndFilterResponse)
                {
                    plans = query
                            .OrderBy(p => request.FinancePlanGuids.IndexOf(p.Guid))
                            .ToList();
                }
                else
                {
                    plans = query
                            .OrderBy(pd => ComparisonOrder[pd.Type])
                            .ThenBy(pd => pd.ProviderID)
                            .ThenBy(pd => pd.TermInYears)
                            .ToList();
                }

                return _loanCalculator.CompareFinancePlans(plans, request, (loanReq, financePlan) =>
                {
                    return _trismartEnhancement.Value.CalculateOption(new OptionCalculatorModel
                    {
                        PlanDefinition = financePlan,
                        Request = loanReq,
                        ScenarioTermInYears = request.ScenarioTermInYears,
                        UtilityInflationRate = request.UtilityInflationRate,
                        PlanName = TrismartProposalEnhancement.SubPlan_Smarter
                    });
                });
            }
        }

        /// <summary>
        /// Quick way of sorting results on comparison. Will probably switch to attributes on FinancePlanType elements to be cleaner.
        /// </summary>
        public static Dictionary<FinancePlanType, int> ComparisonOrder = new Dictionary<FinancePlanType, int>
        {
            { FinancePlanType.None, 1 },
            { FinancePlanType.Cash, 2 },
            { FinancePlanType.Mortgage, 3 },
            { FinancePlanType.Loan, 4 },
            { FinancePlanType.Lease, 5 },
            { FinancePlanType.PPA, 6 },
            { FinancePlanType.Pace, 7 },
            { FinancePlanType.ReverseMortgage, 8 },
        };
    }
}
