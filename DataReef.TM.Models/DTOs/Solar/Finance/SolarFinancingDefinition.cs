using System;

namespace DataReef.TM.Models.DTOs.Solar.Finance
{
    /// <summary>
    /// A financing package that is applied in order ( according to the start month ) for a solar system financing proposal.
    /// </summary>
    public class SolarFinancingDefinition
    {

        public string Name { get; set; }

        public string Key { get; set; }

        /// <summary>
        /// The month ( from the first date of financing for the solar system ) .  Starts with 1
        /// </summary>
        public int StartMonth { get; set; }

        /// <summary>
        /// The lenght of the financing in Month
        /// </summary>
        public int Length { get; set; }

        /// <summary>
        /// APR
        /// </summary>
        public double InterestRate { get; set; }

        public bool IsInterestOnly { get; set; }

        public FinancingType Type { get; set; }

        /// <summary>
        /// If type==1 then what percentage of purchase price is financed
        /// </summary>
        public double CalculationRate
        {
            get;
            set;
        }

        /// <summary>
        /// amount to finance ( pre down payment )
        /// </summary>
        public decimal AmountFinanced { get; set; }

        /// <summary>
        /// Month is in WorldSpace not month of loan.  Must translate Month to MonthOfLoan
        /// </summary>
        /// <param name="month"></param>
        /// <returns></returns>
        public decimal PrincipalForMonth(int month)
        {
            if (month < 1 || month > StartMonth + Length) return 0;
            if (this.IsInterestOnly) return 0;

            int year = (int)Math.Ceiling((double)month / 12);
            int relativeMonth = month - StartMonth + 1;

            if (month >= StartMonth && month <= StartMonth + Length && relativeMonth <= Length)
            {
                return (decimal)Microsoft.VisualBasic.Financial.PPmt((InterestRate / 12.0), relativeMonth, Length, (double)AmountFinanced);
            }

            return 0;
        }

        /// <summary>
        /// Month is in WorldSpace not month of loan.  Must translate Month to MonthOfLoan
        /// </summary>
        /// <param name="month"></param>
        /// <returns></returns>
        public decimal InterestForMonth(int month)
        {
            if (month < 1 || month > StartMonth + Length) return 0;

            if (IsInterestOnly)
            {

                return (decimal)(InterestRate / 12.0 * (double)AmountFinanced);

            }
            else
            {
                int year = (int)Math.Ceiling((double)month / 12);
                int relativeMonth = month - StartMonth + 1;

                if (month >= StartMonth && month <= StartMonth + Length && relativeMonth <= Length)
                {
                    return (decimal)Microsoft.VisualBasic.Financial.IPmt((InterestRate / 12.0), relativeMonth, Length, (double)AmountFinanced);
                }
            }

            return 0;
        }

        public decimal TotalPayment()
        {

            if (IsInterestOnly)
            {

                return (decimal)(InterestRate / 12.0 * (double)AmountFinanced * this.Length);

            }
            else
            {

                decimal ret = 0;

                for (int month = 1; month <= this.Length; month++)
                {
                    int year = (int)Math.Ceiling((double)month / 12);
                    ret += Math.Round(Math.Abs(InterestForMonth(month + this.StartMonth - 1) + PrincipalForMonth(month + this.StartMonth - 1)), 2);
                }

                return ret;

            }
        }

        //used by financial rendering engine
        internal bool IsExpired
        {
            get;
            set;
        }

    }
}
