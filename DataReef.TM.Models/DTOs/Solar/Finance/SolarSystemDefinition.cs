using System;

namespace DataReef.TM.Models.DTOs.Solar.Finance
{
    public class SolarSystemDefinition
    {
        public SolarSystemDefinition()
        {
            this.DRate = .005;
            this.UtilityInflationRate = .034;
        }

        /// <summary>
        /// System Size in W
        /// </summary>
        public int SystemSize { get; set; }

        public double TaxRate { get; set; }

        /// <summary>
        /// First year Production in kWh
        /// </summary>
        public int FirstYearAnnualProduction { get; set; }


        /// <summary>
        /// $/kWh
        /// </summary>
        public decimal UtilityElectricityCost { get; set; }

        /// <summary>
        /// Percentge
        /// </summary>
        public double UtilityInflationRate { get; set; }

        /// <summary>
        /// $/W
        /// </summary>
        public decimal PricePerWattASP { get; set; }

        /// <summary>
        /// Dollar amount
        /// </summary>
        public decimal DownPayment { get; set; }

        /// <summary>
        /// annual system degredation rate
        /// </summary>
        public double DRate { get; set; }


        internal decimal PurchasePrice
        {
            get
            {
                return (this.PricePerWattASP * this.SystemSize) * (decimal)(1 + TaxRate);
            }
        }

        internal decimal PurchasePriceNoTax
        {
            get
            {
                return (this.PricePerWattASP * this.SystemSize);
            }
        }

        internal decimal Tax
        {
            get
            {
                return (this.PricePerWattASP * this.SystemSize) * (decimal)(TaxRate);
            }
        }

        internal decimal GrossAmountFinance
        {
            get
            {
                return this.PurchasePrice - this.DownPayment;
            }
        }

        internal double ProdutionForYear(int year)
        {
            if (year <= 0) return 0;
            if (year == 1) return FirstYearAnnualProduction;

            double lastProduction = FirstYearAnnualProduction;
            double annualProduction = lastProduction;

            for (int x=2;x<= year;x++)
            {
                annualProduction = lastProduction * (1 - DRate);
                lastProduction = annualProduction;       
            }

            return annualProduction;
            
        }

        internal double ProductionForMonth(int month)
        {
            int year = (int)Math.Ceiling((double)month / 12);
            return ProdutionForYear(year) / 12.0;
        }

        internal double UtilityRateForYear(int year)
        {

            if (year <= 0) return 0;
            if (year == 1) return (double)this.UtilityElectricityCost;

            double lastUtilityRate = (double)this.UtilityElectricityCost;
            double currentRate = lastUtilityRate;

            for (int x = 2; x <= year; x++)
            {
                currentRate = lastUtilityRate * (1 + this.UtilityInflationRate);
                lastUtilityRate = currentRate;
            }

            return currentRate;
        }
    }
}
