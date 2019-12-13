namespace DataReef.TM.Models.DTOs.Signatures.Proposals
{
    public class ProposalFinancePlanOption
    {
        public string Name { get; set; }
        public PlanOptionType PlanOptionType { get; set; }
        public string Description { get; set; }
        public decimal Balance { get; set; }
        public int PaymentFactorsFirstPeriod { get; set; } = 18;
        public decimal Payment18M { get; set; }
        public decimal Payment19M { get; set; }
        public decimal NewUtilityBill { get; set; }
        public decimal MonthlySavings { get; set; }
        public decimal AnnualSavings { get; set; }
        public decimal CumulativeSavings3Y { get; set; }
        public decimal CumulativeSavings30Y { get; set; }
        public decimal SolarkWhRate30Y { get; set; }
        public int Breakeven { get; set; }
        public decimal ROI { get; set; }
    }

    public enum PlanOptionType
    {
        Standard,
        Smart,
        Smarter
    }
}
