using DataReef.TM.Models.Enums;

namespace DataReef.TM.Models.DTOs.Solar.Finance
{
    public class DealerIncentive
    {
        public AdderItemRateType RateType { get; set; }
        public AdderItemRecurrenceType RecurrenceType { get; set; }
        public int RecurrenceStart { get; set; }
        public int RecurrencePeriod { get; set; }
        public decimal Amount { get; set; }
        public decimal Quantity { get; set; }

        private int StartMonth => RecurrenceType == AdderItemRecurrenceType.OneTime
            ? 0
            : RecurrenceType == AdderItemRecurrenceType.Monthly
                ? RecurrenceStart
                : ((RecurrenceStart - 1) * 12) + 1;

        private int EndMonth => RecurrenceType == AdderItemRecurrenceType.OneTime
            ? 0
            : RecurrenceType == AdderItemRecurrenceType.Monthly
            ? RecurrenceStart + RecurrencePeriod
            : ((RecurrenceStart + RecurrencePeriod - 1) * 12) + 1;

        public decimal GetIncentiveValue(int monthIndex, long systemSize)
        {
            if (!IncentiveShouldApply(monthIndex))
            {
                return 0;
            }

            return GetTotal(systemSize);
        }

        public decimal GetTotal(long systemSize)
        {
            switch (RateType)
            {
                case AdderItemRateType.Flat:
                    return Amount * Quantity;
                case AdderItemRateType.PerKw:
                    return Amount * Quantity * ((decimal)systemSize / 1000);
                case AdderItemRateType.PerWatt:
                    return Amount * Quantity * systemSize;
            }
            return 0;
        }

        public bool IncentiveShouldApply(int monthIndex)
        {
            return monthIndex >= StartMonth && monthIndex < EndMonth;
        }

        /// <summary>
        /// Get the total incentives for all period
        /// </summary>
        /// <returns></returns>
        public decimal GetGrandTotal(long systemSize)
        {
            if (RecurrenceType == AdderItemRecurrenceType.OneTime)
            {
                return GetTotal(systemSize);
            }

            decimal result = 0;

            for (int i = StartMonth; i < EndMonth; i++)
            {
                result += GetIncentiveValue(i, systemSize);
            }

            return result;
        }
    }
}
