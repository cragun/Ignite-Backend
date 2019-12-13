using System;

namespace DataReef.TM.Models.DTOs.Solar.Proposal
{
    public abstract class AmortizationRow
    {

        /// <summary>
        /// Year of proposal scenario ( 1,2,3,4,...25)
        /// </summary>
        public int Year { get; set; }

        /// <summary>
        /// kwH that the system would produce that month
        /// </summary>
        public int Production { get; set; }

        /// <summary>
        /// kWh that the consumer would use that month
        /// </summary>
        public int Consumption { get; set; }

        /// <summary>
        /// What the consumer would have to pay for electricity that month if no solar
        /// </summary>
        public decimal CostWithoutSolar { get; set; }

        /// <summary>
        /// net amount what the consumer acutally pays that month ( without purchasing any extra needed non solar electricity ) /// </summary>
        public decimal SolarOnlyCosts { get; set; }

        /// <summary>
        /// The cost that will need to be spent to purchase power after solar ( if solar doesnt produce enough )
        /// </summary>
        public decimal PostSolarElectricPurchased { get; set; }

        /// <summary>
        /// net amount what the consumer acutally pays that month ( including purchasing any extra needed non solar electricity )
        /// </summary>
        public decimal CostsWithSolarAndElectric { get; set; }

        /// <summary>
        /// Ending Balance of Solar Loans
        /// </summary>
        public decimal EndingBalance { get; set; }

        /// <summary>
        /// beginning balance of solar loans ( prior months ending balance )
        /// </summary>
        public decimal BeginningBalance { get; set; }

        /// <summary>
        /// Pricipal and Interest paid, does not include an incentives
        /// </summary>
        public decimal Payment { get; set; }

        /// <summary>
        /// Interest
        /// </summary>
        public decimal Interest { get; set; }

        /// <summary>
        /// Principal Paid
        /// </summary>
        public decimal Principal { get; set; }

        public decimal PBI { get; set; }

        public decimal SREC { get; set; }

        public decimal ITC { get; set; }

        public decimal ReinvestedIncentives { get; set; }

        public decimal TotalPaymentsAndReinvestments { get; set; }

        public decimal TotalBenefits
        {
            get
            {
                return PBI + SREC + ITC;
            }
        }

        public decimal TotalCost { get; set; }


        public decimal CumulativeCostOfSolar { get; set; }


        public override string ToString()
        {
            return String.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16}",
                this.Year.ToString(),
                this.Production,
                this.Consumption,
                this.BeginningBalance,
                this.CostWithoutSolar,
                this.SolarOnlyCosts,
                this.CostsWithSolarAndElectric,
                this.Payment,
                this.Principal,
                this.Interest,
                this.TotalBenefits,
                this.ReinvestedIncentives,
                this.TotalPaymentsAndReinvestments,
                this.TotalCost,
                this.CumulativeCostOfSolar,
                this.EndingBalance,
                this.PostSolarElectricPurchased
            );
        }
    }
}
