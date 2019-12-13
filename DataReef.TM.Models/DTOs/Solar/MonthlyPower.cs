
namespace DataReef.TM.Models.DTOs.Solar
{
    /// <summary>
    /// Class for monthly consumption and production information.
    /// </summary>
    public class MonthlyPower
    {
        /// <summary>
        /// Price payed.
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// KiloWatts consumed in a month.
        /// </summary>
        public int ConsumptionInKwh { get; set; }

        /// <summary>
        /// KiloWatts produced in a month by the solar array.
        /// </summary>
        public int ProductionInKwh { get; set; }
    }
}
