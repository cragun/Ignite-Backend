using DataReef.Core;
using DataReef.TM.Models.DTOs.Signatures.Proposals;
using DataReef.TM.Models.DTOs.Solar.Finance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Services.Services.ProposalAddons.Sigora
{
    public class SigoraProposalEnhancer
    {

        public void EnhanceProposalData(Proposal2DataView proposal, ProposalDVConstructor param, bool roundAmounts = false)
        {
            var savingsAffectingIncentives = GetIncentivesForAverages(param.Request);

            if ((savingsAffectingIncentives?.Count ?? 0) == 0)
            {
                return;
            }

            proposal.EnergyCosts.DynamicItems = proposal.EnergyCosts.DynamicItems ?? new List<ProposalEnergyCostItem>();

            decimal firstYearIncentivesTotal = 0;
            for (int i = 1; i <= 12; i++)
            {
                firstYearIncentivesTotal += savingsAffectingIncentives.Sum(inc => inc.GetIncentiveValue(i, param.Request.SystemSize));
            }
            var totalIncentives = savingsAffectingIncentives.Sum(inc => inc.GetGrandTotal(param.Request.SystemSize));

            var request = param.FinancePlan.GetRequest(true);
            var response = param.FinancePlan.GetResponse(true);


            var monthlyBill = param.Response.FirstYearMonthlyElectricityBillWithSolar - (firstYearIncentivesTotal / 12);
            var annualAverageWithSolar = (response.TotalElectricityBillWithSolar - totalIncentives) / request.ScenarioTermInYears;
            var withSigora = new ProposalEnergyCostItem(monthlyBill, annualAverageWithSolar, response.Lcoe, "With Sigora", "with-sigora-averages", roundAmounts);

            proposal.EnergyCosts.DynamicItems.Add(withSigora);

            // Update: With Solar (subtract the incentives from TotalBenefitsAndIncentives to get the LCOE without incentive)
            var withSolar = proposal.EnergyCosts.WithSolar;
            response.TotalBenefitsAndIncentives -= totalIncentives;
            withSolar.CostOfEnergy = (double)response.Lcoe;

            // update Savings
            var withoutSolar = proposal.EnergyCosts.WithoutSolar;
            var savings = proposal.EnergyCosts.Savings;
            savings.MonthlyAverage = withoutSolar.MonthlyAverage - withSigora.MonthlyAverage;
            savings.CostOfEnergy = withoutSolar.CostOfEnergy - withSigora.CostOfEnergy;
            savings.AnnualAverage = withoutSolar.AnnualAverage - withSigora.AnnualAverage;
        }

        private List<Incentive> GetIncentivesForAverages(LoanRequest request)
        {
            return request?
                        .Incentives?
                        .Where(i => i.GetDynamicSettingsWithBoolValue(DynamicIncentive_Names.Sigora_Incentives_UseInSavingsTable, true)?
                                     .Any() == true)?
                        .ToList();
        }

    }
}
