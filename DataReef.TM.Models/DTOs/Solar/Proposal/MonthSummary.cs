
namespace DataReef.TM.Models.DTOs.Solar.Proposal
{
    public class MonthSummary
    {
        public int Year { get; set; }

        public int Month { get; set; }

        /// <summary>
        /// Watts projected that solar system will provide
        /// </summary>
        public decimal Consumption { get; set; }

        /// <summary>
        /// Watts projected that solar system will provide
        /// </summary>
        public decimal Production { get; set; }

        /// <summary>
        /// energy (electric company) cost before solar
        /// </summary>
        public decimal PreSolarCost { get; set; }

        /// <summary>
        /// electric company cost after solar
        /// </summary>
        public decimal PostSolarCost { get; set; }


    }
}
