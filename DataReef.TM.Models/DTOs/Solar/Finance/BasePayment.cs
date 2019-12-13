namespace DataReef.TM.Models.DTOs.Solar.Finance
{
    public abstract class BasePayment
    {
        public int Year { get; set; }

        #region Financial properties

        public decimal PaymentAmount { get; set; }

        /// <summary>
        /// Payment when Federal Tax (ITC) is not applied
        /// </summary>
        public decimal PaymentAmountNoITC { get; set; }

        public decimal InterestCharge { get; set; }

        public decimal Principal { get; set; }

        //  This is the remaining/ending balance after paying the principal => beginning balance = PaymentBalance + Principal
        public decimal PaymentBalance { get; set; }

        public decimal UnpaidInternalBalance { get; set; }

        #endregion

        #region BenefitsAndIncentives

        /// <summary>
        /// The amount of federal money that are being received 
        /// </summary>
        public decimal FederalTaxIncentive { get; set; }

        /// <summary>
        /// The amount of money that are being received from the states that offer this incentive
        /// </summary>
        public decimal StateTaxIncentive { get; set; }

        /// <summary>
        /// Performance-based incentives
        /// </summary>
        public decimal PBI { get; set; }

        /// <summary>
        /// Solar Renewable Energy Credit
        /// </summary>
        public decimal SREC { get; set; }
        public decimal TakeHomeIncentives { get; set; }

        public decimal DealerIncentives { get; set; }

        public decimal TotalBenefitsAndIncentives => FederalTaxIncentive + StateTaxIncentive + PBI + SREC + DealerIncentives;

        #endregion BenefitsAndIncentives

        #region Energy related properties

        public double SystemProduction { get; set; }

        public double Consumption { get; set; }
        public double PostAddersConsumption { get; set; }

        public double NewConsumption => PostAddersConsumption != 0 ? PostAddersConsumption : Consumption;

        public decimal ElectricityBillWithoutSolar { get; set; }

        public decimal ElectricityBillWithSolar { get; set; }

        public decimal Savings => ElectricityBillWithoutSolar + TotalBenefitsAndIncentives - (ElectricityBillWithSolar + PaymentAmount);
        public decimal SavingsNoITC => ElectricityBillWithoutSolar + TotalBenefitsAndIncentives - (ElectricityBillWithSolar + PaymentAmountNoITC);

        public decimal BalanceAndPrincipal => PaymentBalance + Principal;

        public decimal StartBalance { get; set; }
        public decimal EndBalance { get; set; }

        public decimal GetSavings(bool isITCApplied = true)
        {
            return isITCApplied ? Savings : SavingsNoITC;
        }

        #endregion
    }
}
