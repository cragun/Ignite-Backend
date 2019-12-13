
namespace DataReef.TM.Models.DTOs.Solar.Finance
{
    /// <summary>
    /// The amortization table for power purchase agreement.
    /// </summary>
    public class AmortizationTableRowPPA
    {
        public int Month                { get; set; }

        public int Year                 { get; set; }

        public decimal PreSolarCost     { get; set; }

        public decimal PostSolarCost    { get; set; }

        public decimal Production       { get; set; }

        public decimal PPAPayment       { get; set; }

    }
}
