using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.Integrations.LoanPal.Models.LoanPal
{
    public class LoanCalculatorResponse
    {
        public LoanCalculatorResponse()
        {
        }

        [JsonProperty("initialMonthlyPayment")]
        public string InitialMonthlyPayment { get; set; }
        [JsonProperty("adjustedMonthlyPayment")]
        public string AdjustedMonthlyPayment { get; set; }

        [JsonProperty("amortizationTable")]
        public List<AmortizationTableItem> AmortizationTable { get; set; }

        [JsonProperty("error")]
        public string Error { get; set; }

        public decimal InitialMonthlyPaymentValue => GetDecimalValue(InitialMonthlyPayment);

        public decimal AdjustedMonthlyPaymentValue => GetDecimalValue(AdjustedMonthlyPayment);

        private decimal GetDecimalValue(string stringValue)
        {
            decimal value = 0;
            if (decimal.TryParse(stringValue, NumberStyles.Currency, CultureInfo.CurrentCulture.NumberFormat, out value))
            {
                return value;
            }

            return 0;
        }

        /// <summary>
        /// Do some processing. (e.g. remove unused items from AmortizationTable)
        /// </summary>
        public void ProcessData()
        {
            AmortizationTable = AmortizationTable?
                                    .Where(at => at.Period > 0)?
                                    .ToList();
        }
    }

    public class AmortizationTableItem
    {
        public AmortizationTableItem()
        { }

        [JsonProperty("paymentDate")]
        public DateTime PaymentDate { get; set; }

        [JsonProperty("daysInPeriod")]
        public int DaysInPeriod { get; set; }

        [JsonProperty("period")]
        public int Period { get; set; }

        [JsonProperty("paymentNumber")]
        public int PaymentNumber { get; set; }

        [JsonProperty("prepayment")]
        public decimal Prepayment { get; set; }

        [JsonProperty("unpaidInterest")]
        public decimal UnpaidInterest { get; set; }

        [JsonProperty("beginningBalance")]
        public decimal BeginningBalance { get; set; }

        [JsonProperty("monthlyPayment")]
        public decimal MonthlyPayment { get; set; }

        [JsonProperty("interest")]
        public decimal Interest { get; set; }

        [JsonProperty("appliedToUnpaidInterest")]
        public decimal AppliedToUnpaidInterest { get; set; }

        [JsonProperty("principal")]
        public decimal Principal { get; set; }

        [JsonProperty("endBalance")]
        public decimal EndBalance { get; set; }
    }
}
