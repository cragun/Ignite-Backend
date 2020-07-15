using DataReef.TM.Models.DTOs.Solar.Finance;
using DataReef.TM.Models.Enums;
using DataReef.TM.Models.Solar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models.DTOs.Signatures.Proposals
{
    public class ProposalFinancing
    {
        public FinancePlanType FinancingType { get; set; }
        public string Loan { get; set; }
        public string ProviderName { get; set; }
        public double InterestRate { get; set; }
        public double FederalTaxIncentivePercentage { get; set; }
        public double TotalFederalTaxIncentive { get; set; }
        public int Term { get; set; }
        public int TermInMonths { get; set; }
        public string Description { get; set; }
        public double MonthlyPayment { get; set; }
        public decimal DownPayment { get; set; }
        public double LeaseEscalator { get; set; }
        public string RequestJSON { get; set; }

        public ProposalFinancing() { }

        public ProposalFinancing(FinancePlan financePlan, LoanRequest request, LoanResponse response, bool roundAmounts = false)
        {
            var plan = financePlan.FinancePlanDefinition;

            Loan = plan.Name;
            RequestJSON = Newtonsoft.Json.JsonConvert.SerializeObject(request);
            InterestRate = (double)response.StatedApr;
            Term = plan.TermInYears;
            TermInMonths = plan.TermInMonths == 0 ? plan.TermInYears * 12 : plan.TermInMonths;
            Description = plan.Description;
            MonthlyPayment = roundAmounts ? Math.Round((double)response.MonthlyPayment) : (double)response.MonthlyPayment;
            DownPayment = roundAmounts ? Math.Round(request.DownPayment) : request.DownPayment;
            ProviderName = plan?.Provider?.Name;
            FinancingType = plan.Type;
            TotalFederalTaxIncentive = roundAmounts ? Math.Round((double)response.TotalFederalTaxIncentive) : (double)response.TotalFederalTaxIncentive;
            FederalTaxIncentivePercentage = (double)request.FederalTaxIncentivePercentage * 100;
            LeaseEscalator = (double)request.LeaseEscalator;
        }
    }
}
