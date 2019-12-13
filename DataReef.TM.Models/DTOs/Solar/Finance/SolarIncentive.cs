using DataReef.TM.Models.DTOs.Solar.Finance.Enums;
using System;

namespace DataReef.TM.Models.DTOs.Solar.Finance
{
    public class SolarIncentive
    {

        private IncentivePaymentFrequency _frequency;

        public string Name { get; set; }

        public string Key { get; set; }

        public BenefitType BenefitType { get; set; }

        public bool IsUsedToRefi { get; set; }

        public IncentivePaymentFrequency PaymentFrequency
        {
            get
            {
                if (this.BenefitType == BenefitType.UpfrontRebate) return IncentivePaymentFrequency.UpFront; else return _frequency;
            }
            set
            {
                _frequency = value;
            }
        }

        /// <summary>
        /// An absolute dollar to be applied as Incentive
        /// </summary>
        public decimal AbsoluteAmount { get; set; }

        /// <summary>
        /// a percentage to be applied to either production or purchase price to determine the incentive
        /// </summary>
        public decimal RelativeAmount { get; set; }

        /// <summary>
        /// To what number is the RelativeAmount applied in order to derive the Incentive Value. 
        /// </summary>
        public RelativeAmountType RelativeAmountType { get; set; }

        /// <summary>
        /// The month ( from inception ) to start the incentive
        /// </summary>
        public int StartMonth { get; set; }

        /// <summary>
        /// the length in months that the incentive runs
        /// </summary>
        public int Length { get; set; }

        public SolarIncentivesSource Source { get; set; }

        /// <summary>
        /// Month is in WorldSpace not month of Financial Analysis.  Must translate Month to MonthOfLoan
        /// </summary>
        /// <param name="month"></param>
        /// <returns></returns>
        public decimal AmountForMonth(int month)
        {
            int year = (int)Math.Ceiling((double)month / 12);
            int relativeMonth = month - StartMonth;  //this var is zero based, whereas month var is always 1 based
            int monthOfYear = month <= 12 ? month : (month % 12) + 1;
            double annualProduction = (1.0 - (this.Definition.System.DRate * (year - 1))) * this.Definition.System.FirstYearAnnualProduction;
            double monthlyProduction = annualProduction / 12.0;


            if (month >= StartMonth && month < StartMonth + Length)
            {
                if (PaymentFrequency == IncentivePaymentFrequency.Monthly)
                {
                    if (this.RelativeAmountType == RelativeAmountType.None)
                    {
                        return Math.Round(this.AbsoluteAmount, 2);
                    }
                    else if (this.RelativeAmountType == RelativeAmountType.PerWatt)
                    {
                        return Math.Round(this.Definition.System.SystemSize * this.RelativeAmount, 2);
                    }
                    else if (this.RelativeAmountType == RelativeAmountType.TotalPurchasePrice)
                    {
                        return Math.Round(this.RelativeAmount * this.Definition.System.PurchasePrice, 2);
                    }
                    else if (this.RelativeAmountType == RelativeAmountType.PriorYearProduction)
                    {
                        if (year > 1) return Math.Round((decimal)this.Definition.System.ProdutionForYear(year - 1) * this.RelativeAmount, 2);
                    }
                    else if (this.RelativeAmountType == RelativeAmountType.PriorMonthProduction)
                    {
                        if (month > 1) return Math.Round((decimal)this.Definition.System.ProductionForMonth(month - 1) * this.RelativeAmount, 2);
                    }
                }
                else if (PaymentFrequency == IncentivePaymentFrequency.FirstOfYear && monthOfYear == 1)
                {

                    if (this.RelativeAmountType == RelativeAmountType.None)
                    {
                        return Math.Round(this.AbsoluteAmount, 2);
                    }
                    else if (this.RelativeAmountType == RelativeAmountType.PerWatt)
                    {
                        return Math.Round(this.Definition.System.SystemSize * this.RelativeAmount, 2);
                    }

                    else if (this.RelativeAmountType == RelativeAmountType.TotalPurchasePrice)
                    {
                        return Math.Round(this.RelativeAmount * this.Definition.System.PurchasePrice, 2);
                    }
                    else if (this.RelativeAmountType == RelativeAmountType.PriorYearProduction)
                    {
                        if (year > 1) return Math.Round((decimal)this.Definition.System.ProdutionForYear(year - 1) * this.RelativeAmount, 2);
                    }
                    else if (this.RelativeAmountType == RelativeAmountType.PriorMonthProduction)
                    {
                        if (month > 1) return Math.Round((decimal)this.Definition.System.ProductionForMonth(month - 1) * this.RelativeAmount, 2);
                    }
                }
            }

            return 0;
        }

        /// <summary>
        /// rebates calculated before the first amortization... will be used to pay down system
        /// </summary>
        /// <returns></returns>
        public decimal AmountUpFront()
        {
            if (PaymentFrequency != IncentivePaymentFrequency.UpFront) return 0;

            int year = 1;
            double annualProduction = (1.0 - (this.Definition.System.DRate * (year - 1))) * this.Definition.System.FirstYearAnnualProduction;
            double monthlyProduction = annualProduction / 12.0;

            if (this.RelativeAmountType == RelativeAmountType.None)
            {
                return Math.Round(this.AbsoluteAmount, 2);
            }
            else if (this.RelativeAmountType == RelativeAmountType.TotalPurchasePrice)
            {
                return Math.Round(this.RelativeAmount * this.Definition.System.PurchasePrice, 2);
            }


            return 0;
        }

        internal LoanRequestSunEdison Definition
        {
            get;
            set;
        }

    }
}
