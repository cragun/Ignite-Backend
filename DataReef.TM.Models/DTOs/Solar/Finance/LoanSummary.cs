using DataReef.TM.Models.DTOs.Solar.Finance;
using DataReef.TM.Models.DTOs.Solar.Proposal;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataReef.TM.Models.DTOs.Solar.Finance
{
    public class LoanSummary
    {
        public LoanSummary()
        {
            this.Details = new List<LoanSummaryItem>();
        }

        /// <summary>
        /// Total length of all financing
        /// </summary>
        public decimal Months { get; set; }

        public decimal OneYearUtilityCostsNoSolar { get; set; }

        public decimal TotalUtilityCostsNoSolar { get; set; }

        public decimal PurchasePrice { get; set; }

        public decimal Tax { get; set; }

        public decimal PurchasePriceWithTax { get; set; }

        public decimal TotalBenefits { get; set; }

        public decimal NetInvestment { get; set; }

        public decimal DownPayment { get; set; }

        public double StatedAPR { get; set; }

        public double EffectiveAPR { get; set; }

        public decimal PrincipalPaid { get; set; }

        public decimal InterestPaid { get; set; }

        public decimal ReinvestedIncentives { get; set; }

        [Obsolete("Use PurchasePriceWithTax",true)]
        public decimal PurchasePricePlusTax { get; set; }

        public decimal LoanAmount { get; set; }

        public decimal ITCTotal { get; set; }

        public decimal PBITotal { get; set; }

        public decimal SRECTotal { get; set; }

        public decimal RebateTotal { get; set; }


        public ICollection<LoanSummaryItem> Details { get; set; }

        public static LoanSummary Create(LoanRequestSunEdison request, AmortizationTable table)
        {

            LoanSummary ls = new LoanSummary();
            ls.DownPayment = request.System.DownPayment;
            ls.EffectiveAPR = 0;
            ls.Months = request.Months;
            ls.Tax = request.System.Tax;
            ls.PurchasePrice = request.System.PurchasePriceNoTax;
            ls.PurchasePriceWithTax = ls.Tax + ls.PurchasePrice;
            ls.TotalBenefits = table.Rows.Sum(r => r.TotalBenefits);
            ls.TotalUtilityCostsNoSolar = table.Rows.Sum(r => r.CostWithoutSolar);
            ls.OneYearUtilityCostsNoSolar = table.Rows.Where(rr => rr.Year == 1).Sum(r => r.CostWithoutSolar);
            ls.NetInvestment = ls.PurchasePriceWithTax - ls.TotalBenefits;
            ls.ITCTotal = table.Rows.Sum(rr => rr.ITC);
            ls.SRECTotal = table.Rows.Sum(rr => rr.SREC);
            ls.PBITotal = table.Rows.Sum(rr => rr.PBI);
            ls.RebateTotal = table.Rebates;
            ls.TotalBenefits = table.Rows.Sum(rr => rr.TotalBenefits);
            ls.StatedAPR = request.Financing.Any() ? request.Financing .OrderByDescending(f=> f.Length).First().InterestRate : 0;
            ls.EffectiveAPR = ls.StatedAPR;
            ls.PrincipalPaid = table.Rows.Sum(r => r.Principal);
            ls.InterestPaid = table.Rows.Sum(r => r.Interest);
            ls.ReinvestedIncentives = table.Rows.Sum(r => r.ReinvestedIncentives);
            ls.LoanAmount = request.Financing.Sum(ff => ff.AmountFinanced);


            if (request.Financing != null)
            {
                foreach (SolarFinancingDefinition def in request.Financing.OrderBy(f => f.StartMonth))
                {
                    LoanSummaryItem lsi = new LoanSummaryItem();
                    lsi.Name = def.Name;
                    lsi.Length = def.Length;
                    lsi.Amount = def.AmountFinanced;
                    lsi.InterestRate = (float)def.InterestRate;
                    lsi.TotalPayments = def.TotalPayment();
                    lsi.AvgMonthlyPayment = lsi.TotalPayments / def.Length;
                    ls.Details.Add(lsi);
                }
            }

            return ls;
        }

    }
}
