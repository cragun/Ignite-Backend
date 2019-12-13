using DataReef.TM.Models.DTOs.Solar.Finance;
using DataReef.TM.Models.DTOs.Solar.Proposal;
using System;
using System.Linq;

namespace DataReef.TM.Models.DTOs.Solar.Finance
{
    public class LCOESummary
    {
        public LCOESummary()
        {
        }

        public decimal PerKiloWattWithSolar { get; set; }

        public decimal PerKiloWattWithoutSolar { get; set; }

        public decimal TotalElectricCostsWithoutSolar { get; set; }

        public decimal TotalSolarOnlyCosts { get; set; }

        public decimal PurchasePrice { get; set; }

        public decimal Tax { get; set; }

        public decimal PurchasePricePlusTax { get; set; }

        public decimal TotalITCs { get; set; }

        public decimal TotalSRECs { get; set; }

        public decimal TotalRebates { get; set; }

        public decimal TotalPBIs { get; set; }

        public decimal NetPurchasePrice { get; set; }

        public decimal TotalSavings { get; set; }

        public decimal TotalIncentives { get; set; }


        public double TermLengthInYears { get; set; }

        public int TermLengthInMonths { get; set; }


        public static LCOESummary Create(LoanRequestSunEdison definition, AmortizationTable amortization)
        {
            LCOESummary ret = new LCOESummary();
            ret.PerKiloWattWithSolar = Math.Round(amortization.Rows.Sum(r => r.SolarOnlyCosts) / amortization.Rows.Sum(r => r.Production), 2);
            ret.PerKiloWattWithoutSolar = Math.Round(amortization.Rows.Sum(r => r.CostWithoutSolar) / amortization.Rows.Sum(r => r.Production), 2);

            var production = amortization.Rows.Sum(r => r.Production);

            ret.TotalElectricCostsWithoutSolar = amortization.Rows.Sum(r => r.CostWithoutSolar);
            ret.TotalSolarOnlyCosts = amortization.Rows.Sum(r => r.SolarOnlyCosts);
            ret.PerKiloWattWithSolar = ret.TotalSolarOnlyCosts / production;
            ret.PerKiloWattWithoutSolar = ret.TotalElectricCostsWithoutSolar / production;
            ret.PurchasePrice = definition.System.PurchasePriceNoTax;
            ret.Tax = definition.System.Tax;
            ret.PurchasePricePlusTax = definition.System.PurchasePrice + definition.System.Tax;
            ret.TotalITCs = amortization.Rows.Sum(r => r.ITC);
            ret.TotalSRECs = amortization.Rows.Sum(r => r.SREC);
            ret.TotalRebates = amortization.Rebates;
            ret.TotalPBIs = amortization.Rows.Sum(r => r.PBI);
            ret.TotalIncentives = ret.TotalRebates + ret.TotalPBIs + ret.TotalITCs + ret.TotalSRECs;
            ret.NetPurchasePrice = ret.PurchasePricePlusTax - ret.TotalIncentives;
            ret.TotalSavings = ret.TotalElectricCostsWithoutSolar - (ret.NetPurchasePrice + amortization.Rows.Sum(r => r.Interest));
            ret.TermLengthInMonths = definition.Months;
            ret.TermLengthInYears = ret.TermLengthInMonths / 12.0;

            return ret;
        }
    }
}
