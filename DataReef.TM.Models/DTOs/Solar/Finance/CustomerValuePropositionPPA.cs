
namespace DataReef.TM.Models.DTOs.Solar.Finance
{

    /// <summary>
    /// Customer value proposition for power purchase agreement.
    /// </summary>
    public class CustomerValuePropositionPPA
    {
        public decimal PricePerKWH { get; set; }

        public decimal FirstYearSavings { get; set; }

        public decimal FirstYearCost { get; set; }
    }
}
