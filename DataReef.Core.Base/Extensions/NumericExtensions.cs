using System;

namespace DataReef.Core.Extensions
{
    public static class NumericExtensions
    {
        public static decimal PlusTax(this decimal value, decimal taxRate)
        {
            return value * (1 + taxRate);
        }

        public static double PlusTax(this double value, double taxRate)
        {
            return value * (1 + taxRate);
        }

        /// <summary>
        /// Returns the year for the given month index
        /// </summary>
        /// <param name="month">Month index (1 based)</param>
        /// <returns></returns>
        public static int GetYear(this int month)
        {
            return (int)Math.Ceiling((double)month / 12);
        }

        /// <summary>
        /// Method that returns the Calendar month (0 - 11) for a given month index.
        /// It uses the current server month as offset.
        /// </summary>
        /// <param name="month"></param>
        /// <returns>Calendaristic month (0 - 11)</returns>
        public static int GetCalendarMonth(this int month)
        {
            int currentServerMonth = DateTime.UtcNow.Month;
            // get the actual calendaristic month
            int calendarMonth = (currentServerMonth + month - 1) % 12;
            // we normalize it, because the currentServerMonth we're always one month ahead
            calendarMonth = calendarMonth == 0 ? 11 : calendarMonth - 1;

            return calendarMonth;
        }

        /// <summary>
        /// Apply a annual variation rate to a given value.
        /// Will convert the annual variation rate to monthly variation rate before applying it.
        /// </summary>
        /// <param name="value">Given value</param>
        /// <param name="yearIndex">Index of the year we're computing the value for</param>
        /// <param name="variation">Annual variation rate</param>
        /// <param name="add">true to add the rate to the value, false to subtract it from the value</param>
        /// <param name="increasePercentage">Percentage used to increase the initial production. Used on Lease Finance plans.</param>
        /// <returns></returns>
        public static decimal ApplyVariation(this decimal value, int yearIndex, decimal variation, bool add, decimal increasePercentage)
        {
            if (yearIndex == 1)
            {
                return value.AddPercentage(increasePercentage);
            }

            var sign = add ? 1 : -1;
            var result = value.AddPercentage(increasePercentage); ;
            var appliedVariation = sign * variation;

            for (int i = 2; i <= yearIndex; i++)
            {
                result = result * (1 + appliedVariation);
            }

            return result;
        }

        /// <summary>
        /// Round currency to two decimal places
        /// </summary>
        /// <param name="value"></param>
        /// <param name="decimals">Number of decimals to round the value to</param>
        /// <returns></returns>
        public static double RoundValue(this double value, int decimals = 2)
        {
            return (double)RoundValue((decimal)value, decimals);
        }

        /// <summary>
        /// Round currency to two decimal places
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static decimal RoundValue(this decimal value, int decimals = 2)
        {
            return Math.Round(value, decimals);
        }

        public static decimal AddPercentage(this decimal value, decimal percentage)
        {
            return value + (value * percentage / 100);
        }
    }
}
