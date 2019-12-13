using DataReef.TM.Models.DTOs.Solar.Finance;
using DataReef.TM.Models.DTOs.Solar.Finance.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataReef.TM.Models.DTOs.Solar.Proposal
{
    public class AmortizationTable
    {
        public AmortizationTable()
        {
            this.Rows = new List<AmortizationMonth>();
        }


        public static AmortizationTable Create(LoanRequestSunEdison definition)
        {

            //hook up the circular reference.  bad, I know
            if (definition.Incentives != null) definition.Incentives.ToList().ForEach(ii => ii.Definition = definition);

            //set all cacluated financing AmountFinanced
            if (definition.Financing != null)
            {
                definition.Financing
                .Where(dd => dd.Type == FinancingType.Calculated && dd.CalculationRate > 0)
                .ToList()
                .ForEach(dd => dd.AmountFinanced = definition.System.GrossAmountFinance * (decimal)dd.CalculationRate);
            }

            AmortizationMonth lastRow = null;

            AmortizationTable at = new AmortizationTable();
            decimal loanAmount = definition.System.GrossAmountFinance;

            decimal upFrontRebates = 0;
            decimal reinvestments = 0;

            if (definition.Incentives != null) definition.Incentives.Where(ii => ii.PaymentFrequency == IncentivePaymentFrequency.UpFront).ToList().ForEach(ii => upFrontRebates += ii.AmountUpFront());

            for (int month = 1; month <= definition.Months; month++)
            {
                reinvestments = 0;
                int year = (int)Math.Ceiling((double)month / 12);
                int monthOfYear = month <= 12 ? month : (month % 12) + 1;

                double monthlyProduction = definition.System.ProductionForMonth(month);
                decimal currentUtilityRate = (decimal)definition.System.UtilityRateForYear(year);
                int monthlyConsumption = definition.Consumption.Select(c => c.ConsumptionInKwh).ToArray()[monthOfYear - 1];
                decimal postSolarEletricityPurchased = monthlyConsumption > monthlyProduction ?
                    ((decimal)(monthlyConsumption - monthlyProduction) * currentUtilityRate) : 0;

                decimal interest = 0;
                decimal principal = 0;
                decimal incentives = 0;
                decimal srecs = 0;
                decimal pbis = 0;
                decimal itcs = 0;

                //first, if its the first month, find any financing that has a 0 amount and set it
                if (month == 1 && definition.Financing != null)
                {
                    definition.Financing.Where(ff => ff.AmountFinanced == 0 && ff.IsExpired == false && ff.StartMonth == 1).ToList().ForEach(ff => ff.AmountFinanced = definition.System.GrossAmountFinance);
                }


                //find all the refis - financing pacakges with a type of Refi that starts in the month ( not the first month ).  There cannot be mutliple 
                if (month > 1 && definition.Financing != null && definition.Financing.Any())
                {
                    var refis = definition.Financing.Where(ff => ff.Type == FinancingType.Refi && ff.StartMonth == month && ff.IsExpired == false);

                    if (refis.Count() > 1)
                    {
                        throw new ApplicationException("There cannot be more than one refi in the same month");
                    }

                    if (refis.Any())
                    {
                        var refi = refis.First();

                        decimal refiAmount = lastRow.EndingBalance - reinvestments;
                        refi.AmountFinanced = refiAmount;

                        // now find all other financing and expire
                        definition.Financing.Where(ff => ff.StartMonth < month && ff.IsExpired == false).ToList().ForEach(ff => ff.IsExpired = true);

                        //wipe out reinvestments
                        reinvestments = 0;
                    }
                }

                if (definition.Financing != null) definition.Financing.Where(ff => ff.IsExpired == false).ToList().ForEach(ff => interest += Math.Round(Math.Abs(ff.InterestForMonth(month)), 2));
                if (definition.Financing != null) definition.Financing.Where(ff => ff.IsExpired == false).ToList().ForEach(ff => principal += Math.Round(Math.Abs(ff.PrincipalForMonth(month)), 2));
                if (definition.Incentives != null) definition.Incentives.ToList().ForEach(ii => incentives += Math.Round(ii.AmountForMonth(month), 2));

                if (definition.Incentives != null) definition.Incentives.Where(ii => ii.BenefitType == BenefitType.SREC).ToList().ForEach(ii => srecs += ii.AmountForMonth(month));
                if (definition.Incentives != null) definition.Incentives.Where(ii => ii.BenefitType == BenefitType.PBI).ToList().ForEach(ii => pbis += ii.AmountForMonth(month));
                if (definition.Incentives != null) definition.Incentives.Where(ii => ii.BenefitType == BenefitType.ITC).ToList().ForEach(ii => itcs += ii.AmountForMonth(month));
                if (definition.Incentives != null) definition.Incentives.Where(ii => ii.IsUsedToRefi).ToList().ForEach(ii => reinvestments += ii.AmountForMonth(month));

                at.Rebates = upFrontRebates;

                AmortizationMonth ar = new AmortizationMonth();
                ar.Year = year;
                ar.Month = month;
                ar.BeginningBalance = month == 1 ? loanAmount : at.Rows[month - 2].EndingBalance;
                ar.Interest = interest;
                ar.Principal = principal;
                ar.ReinvestedIncentives = reinvestments;
                ar.PBI = pbis;
                ar.SREC = srecs;
                ar.ITC = itcs;

                ar.Production = (int)monthlyProduction;
                ar.Consumption = monthlyConsumption;

                //ar.CostWithoutSolar = ar.Consumption * currentUtilityRate;
                ar.CostWithoutSolar = (decimal)(monthlyProduction * (double)currentUtilityRate); //cost if we purchased what the panels produced, from the electric company

                ar.CostsWithSolarAndElectric = principal + interest - incentives + postSolarEletricityPurchased;
                ar.PostSolarElectricPurchased = postSolarEletricityPurchased;
                ar.Payment = principal + interest;
                ar.TotalPaymentsAndReinvestments = ar.ReinvestedIncentives + ar.Payment;
                ar.SolarOnlyCosts = principal + interest - incentives; // + ar.ReinvestedIncentives;
                ar.TotalCost = ar.CostsWithSolarAndElectric;
                ar.CumulativeCostOfSolar = month == 1 ? principal + interest - incentives + reinvestments : lastRow.CumulativeCostOfSolar + principal + interest - incentives + reinvestments;
                ar.EndingBalance = ar.BeginningBalance - ar.Principal - reinvestments;

                lastRow = ar;
                at.Rows.Add(ar);
            }

            return at;
        }


        public decimal Rebates { get; set; }

        public List<AmortizationMonth> Rows { get; set; }

        public override string ToString()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            foreach (AmortizationRow row in this.Rows)
            {
                sb.AppendLine(row.ToString());
            }
            return sb.ToString();
        }
    }
}
