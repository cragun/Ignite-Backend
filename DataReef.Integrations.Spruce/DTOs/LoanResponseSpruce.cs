using Newtonsoft.Json;
using System.Collections.Generic;

namespace DataReef.Integrations.Spruce.DTOs
{
    public class LoanResponseSpruce
    {
        /// <summary>
        /// Total amount that will be financed.
        /// </summary>
        public decimal AmountFinanced { get; set; }

        /// <summary>
        /// Monthly payment amount during the promotion period.
        /// </summary>
        public decimal PromoMonthlyPayment { get; set; }
        
        /// <summary>
        /// Monthly payment after promo period, without qualified prepayment
        /// </summary>
        public decimal MonthlyPaymentAfterPromoNoQualifiedPrepayment { get; set; }

        /// <summary>
        /// Monthly payment after promo period, with qualified prepayment
        /// </summary>
        public decimal MonthlyPaymentAfterPromoWithQualifiedPrepayment { get; set; }

        /// <summary>
        /// Payment amount with different terms
        /// </summary>
        public List<PaymentOption> PaymentOptions { get; set; }

        /// <summary>
        /// The amount of federal money that are being received 
        /// </summary>
        public decimal FederalTaxIncentive { get; set; }

        /// <summary>
        /// The amount of money that are being received from the states that offer this incentive
        /// </summary>
        public decimal StateTaxIncentive { get; set; }

        /// <summary>
        /// Calculator disclaimer message
        /// </summary>
        public string Disclaimer { get; set; }

        /// <summary>
        /// The number of months without payment at the beginning of the contract
        /// </summary>
        public int OffsetMonths { get; set; }

        public long TotalSystemProduction { get; set; }

        public decimal TotalElectricityBillWithoutSolar { get; set; }

        public decimal TotalElectricityBillWithSolar { get; set; }

        public decimal TotalSolarPaymentsCost { get; set; }

        public decimal TotalSavings { get; set; }

        public List<PaymentMonth> Months { get; set; }

        public List<PaymentYear> Years { get; set; }

        /// <summary>
        /// The unique identifier for the request as defined by the client request
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Key { get; set; }

    }
}
