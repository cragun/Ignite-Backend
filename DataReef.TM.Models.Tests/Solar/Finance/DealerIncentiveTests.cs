using DataReef.TM.Models.DTOs.Solar.Finance;
using DataReef.TM.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace DataReef.TM.Models.Tests.Solar.Finance
{

    public class DealerIncentiveTests
    {
        /// <summary>
        /// Test DealerIncentive.GetTotal()
        /// </summary>
        /// <param name="rateType"></param>
        /// <param name="amount"></param>
        /// <param name="quantity"></param>
        /// <param name="systemSize">System size in Watts</param>
        /// <param name="total">Expected total calculated by DealerIncentive</param>
        [Theory]
        [InlineData(AdderItemRateType.Flat, 150, 3, 0, 450)]
        [InlineData(AdderItemRateType.PerKw, 150, 3, 10000, 4500)]
        public void Test_GetTotal(AdderItemRateType rateType, decimal amount, decimal quantity, int systemSize, decimal total)
        {
            // Arrange
            var incentive = new DealerIncentive
            {
                RateType = rateType,
                Amount = amount,
                Quantity = quantity
            };

            // Act
            var totalCalculated = incentive.GetTotal(systemSize);

            // Assert

            Assert.Equal(total, totalCalculated);
        }

        [Theory]
        [InlineData(AdderItemRecurrenceType.OneTime, 0, 0, 2, false)]
        [InlineData(AdderItemRecurrenceType.OneTime, 0, 0, 1, false)]
        [InlineData(AdderItemRecurrenceType.Monthly, 0, 0, 1, false)]
        [InlineData(AdderItemRecurrenceType.Monthly, 1, 1, 1, true)]
        [InlineData(AdderItemRecurrenceType.Monthly, 1, 1, 2, false)]
        [InlineData(AdderItemRecurrenceType.Monthly, 1, 12, 12, true)]
        [InlineData(AdderItemRecurrenceType.Monthly, 2, 2, 1, false)]
        [InlineData(AdderItemRecurrenceType.Monthly, 2, 2, 3, true)]
        [InlineData(AdderItemRecurrenceType.Monthly, 2, 2, 4, false)]
        [InlineData(AdderItemRecurrenceType.Yearly, 0, 0, 1, false)]
        [InlineData(AdderItemRecurrenceType.Yearly, 1, 1, 1, true)]
        [InlineData(AdderItemRecurrenceType.Yearly, 1, 1, 12, true)]
        [InlineData(AdderItemRecurrenceType.Yearly, 1, 1, 13, false)]
        [InlineData(AdderItemRecurrenceType.Yearly, 2, 2, 13, true)]
        [InlineData(AdderItemRecurrenceType.Yearly, 2, 2, 24, true)]
        [InlineData(AdderItemRecurrenceType.Yearly, 2, 2, 36, true)]
        [InlineData(AdderItemRecurrenceType.Yearly, 2, 2, 37, false)]
        public void Test_IncentiveShouldApply(AdderItemRecurrenceType recType, int start, int period, int monthIndex, bool shouldApply)
        {
            // Arrange
            var incentive = new DealerIncentive
            {
                RecurrenceType = recType,
                RecurrenceStart = start,
                RecurrencePeriod = period,
            };

            // Act
            var result = incentive.IncentiveShouldApply(monthIndex);

            // Assert
            Assert.Equal(shouldApply, result);
        }

        [Theory]
        [InlineData(AdderItemRecurrenceType.OneTime, AdderItemRateType.Flat, 0, 0, 150, 3, 10000, 450)]
        [InlineData(AdderItemRecurrenceType.OneTime, AdderItemRateType.PerKw, 0, 0, 150, 3, 10000, 4500)]
        [InlineData(AdderItemRecurrenceType.Monthly, AdderItemRateType.Flat, 1, 5, 150, 3, 10000, 2250)]
        [InlineData(AdderItemRecurrenceType.Monthly, AdderItemRateType.PerKw, 1, 5, 150, 3, 10000, 22500)]
        [InlineData(AdderItemRecurrenceType.Monthly, AdderItemRateType.Flat, 3, 5, 150, 3, 10000, 2250)]
        [InlineData(AdderItemRecurrenceType.Monthly, AdderItemRateType.PerKw, 3, 5, 150, 3, 10000, 22500)]
        [InlineData(AdderItemRecurrenceType.Yearly, AdderItemRateType.Flat, 1, 5, 150, 3, 10000, 27000)]
        [InlineData(AdderItemRecurrenceType.Yearly, AdderItemRateType.PerKw, 1, 5, 150, 3, 10000, 270000)]
        [InlineData(AdderItemRecurrenceType.Yearly, AdderItemRateType.Flat, 4, 5, 150, 3, 10000, 27000)]
        [InlineData(AdderItemRecurrenceType.Yearly, AdderItemRateType.PerKw, 4, 5, 150, 3, 10000, 270000)]
        public void Test_GetGrandTotal(AdderItemRecurrenceType recType, AdderItemRateType rateType, int start, int period, decimal amount, decimal quantity, int systemSize, decimal grandTotal)
        {
            // Arrange
            var incentive = new DealerIncentive
            {
                RecurrenceType = recType,
                RateType = rateType,
                RecurrenceStart = start,
                RecurrencePeriod = period,
                Amount = amount,
                Quantity = quantity,
            };

            // Act
            var result = incentive.GetGrandTotal(systemSize);

            // Assert
            Assert.Equal(grandTotal, result);
        }
    }
}
