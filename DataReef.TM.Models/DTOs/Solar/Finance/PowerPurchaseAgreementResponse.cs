
namespace DataReef.TM.Models.DTOs.Solar.Finance
{
    /// <summary>
    /// The response power purchase agreement. 
    /// </summary>
    public class PowerPurchaseAgreementResponse
    {
        public AmortizationTableRowPPA[] AmortizationTable          { get; set; }

        public CustomerValuePropositionPPA CustomerValueProposition { get; set; }

        public bool HasMinimumYield                                 { get; set; }

        public int MinimumYield                                     { get; set; }
    }
}
