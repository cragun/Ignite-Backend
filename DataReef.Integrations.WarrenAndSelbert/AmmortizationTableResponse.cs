using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace DataReef.Integrations.WarrenAndSelbert
{
    public class AmmortizationTableResponse
    {
        public decimal DiscountRate { get; set; }
        public decimal ElectricDegradationYears1To25 { get; set; }
        public decimal ElectricDegradationYears26To30 { get; set; }
        public DateTime StartDate { get; set; }
        public int PISTimingLagInMonths { get; set; }
        public int BillingTimingLagInMonths { get; set; }
        public int RebateLagInMonths { get; set; }
        public int SRECLagInMonths { get; set; }
        public int PBILagInMonths { get; set; }
        public decimal ConsumerDefaultRate { get; set; }
        public int LeaseTermInYears { get; set; }
        public decimal AppraisedValueByState { get; set; }
        public decimal ResidualInput { get; set; }
        public int LeaseExtensionTo { get; set; }
        public decimal OAndMInput { get; set; }
        public decimal OAndMEscRate { get; set; }
        public decimal TotalInsuredValue { get; set; }
        public decimal InsuranceEscRate { get; set; }
        public decimal LiabilityRate { get; set; }
        public decimal LiabilityRateEsc { get; set; }
        public decimal PropertyDamageCoverage { get; set; }
        public List<decimal> AssetManagementInput { get; set; }
        public decimal AssetManagementEscRate { get; set; }
        public decimal SRECAssumptions { get; set; }
        public decimal MaximumSolarSystemPremiumRatio { get; set; }
        public decimal InvestmentTaxCredit { get; set; }
        public decimal CorporateTaxRate { get; set; }
        public decimal WACC { get; set; }
        public decimal NonAutoPayAmount { get; set; }
        public decimal DownPayment { get; set; }
        public string SolarWaterHeaterBackup { get; set; }
        public int UsefulLife { get; set; }
        public decimal BadDebtInput { get; set; }
        public decimal EnergyValue { get; set; }
        public decimal ASPPerWGuess { get; set; }
        public decimal MarginAmount { get; set; }
        public bool PPAAllowed { get; set; }
        public List<decimal> CustomerLeasePayments { get; set; }
        public List<decimal> TerminationValues { get; set; }
        public decimal ConsumerLeaseFullPrepayment { get; set; }
        public decimal DealerASPPerW { get; set; }
        public decimal DealerASP { get; set; }
        public DateTime PISDate { get; set; }
        public DateTime FirstLeasePaymentDate { get; set; }
        public decimal NetSolarCost { get; set; }
        public decimal SolarSystemPremiumRatio { get; set; }
        public string PremiumRatioTest { get; set; }
        public decimal MaxPPARate { get; set; }
        public decimal TotalPaymentsYears1To20 { get; set; }
        public decimal TotalPaymentsYears21To30 { get; set; }
        public decimal TotalPaymentsYears1To30 { get; set; }
        public List<decimal> EstimatedPricePerKWhAnnual { get; set; }
        public List<decimal> EstimatedPricePerKWhLeaseTermWeightedAverageOver20years { get; set; }
        public List<int> YearNumbers { get; set; }
        public List<string> PeriodicityOfPayments { get; set; }
        public List<decimal> PPAPaymentsNoAutoPay { get; set; }
        public List<decimal> SalesUseTaxesNoAutoPay { get; set; }
        public List<decimal> TotalMonthlyPaymentsNoAutoPay { get; set; }
        public List<decimal> PPAPaymentsYesAutoPay { get; set; }
        public List<decimal> SalesUseTaxesYesAutoPay { get; set; }
        public List<decimal> TotalMonthlyPaymentsYesAutoPay { get; set; }
        public decimal EstimatedTaxesOnDownPayment { get; set; }
        public decimal FirstYearMonthlyLeasePayment { get; set; }
        public decimal FirstYearMonthlySalesUseTaxPayment { get; set; }
        public decimal TotalSalesUseTaxPaidNoAutoPay { get; set; }
        public decimal TotalSalesUseTaxPaidYesAutoPay { get; set; }
        public decimal TotalLeasePaymentsNoAutoPay { get; set; }
        public decimal TotalLeasePaymentsYesAutoPay { get; set; }
        public List<decimal> AnnualOutputSchedule { get; set; }
        public decimal TotalAdjustedCost { get; set; }
        public decimal InitialAnnualRevenueYr1Amount { get; set; }
        public decimal ExtendedAnnualRevenueYr21Amount { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal EffectiveAfterTaxYield { get; set; }
        public decimal AdjustedASPPerW { get; set; }
        public decimal TotalRebate { get; set; }
        public string CustomerUniqueID { get; set; }
        public string ProposalID { get; set; }
        public string ModSolarCallVersionID { get; set; }
        public string UniqueFinancialRunIdentifier { get; set; }
        public string FinancialModelVersion { get; set; }
        public string Timestamp { get; set; }

        public AmmortizationTableResponse(XDocument xml)
        {
            var outputElements = xml.Element("abc").Element("batchoutput").Elements("output");
            DiscountRate = Decimal.Parse(GetValue(outputElements, "Discount Rate").Replace("%", ""));
            ElectricDegradationYears1To25 = Decimal.Parse(GetValue(outputElements, "Electric Degradation (Yr 1-25)").Replace("%", ""));
            ElectricDegradationYears26To30 = Decimal.Parse(GetValue(outputElements, "Electric Degradation (Yr 26-30)").Replace("%", ""));
            StartDate = DateTime.Parse(GetValue(outputElements, "Start Date"));
            PISTimingLagInMonths = int.Parse(GetValue(outputElements, "PIS Timing Lag"));
            BillingTimingLagInMonths = int.Parse(GetValue(outputElements, "Billing Timing Lag"));
            RebateLagInMonths = int.Parse(GetValue(outputElements, "Rebate Lag"));
            SRECLagInMonths = int.Parse(GetValue(outputElements, "SREC Lag"));
            PBILagInMonths = int.Parse(GetValue(outputElements, "PBI Lag"));
            ConsumerDefaultRate = Decimal.Parse(GetValue(outputElements, "Consumer Default Rate").Replace("%", ""));
            LeaseTermInYears = int.Parse(GetValue(outputElements, "Lease Term"));
            AppraisedValueByState = Decimal.Parse(GetValue(outputElements, "Appraised value by state / Max ASP"));
            ResidualInput = Decimal.Parse(GetValue(outputElements, "Residual Input").Replace("%", ""));
            LeaseExtensionTo = int.Parse(GetValue(outputElements, "Lease Extension To"));
            OAndMInput = Decimal.Parse(GetValue(outputElements, "O and M Input"));
            OAndMEscRate = Decimal.Parse(GetValue(outputElements, "O and M Esc Rate").Replace("%", ""));
            TotalInsuredValue = Decimal.Parse(GetValue(outputElements, "Total Insured Value"));
            InsuranceEscRate = Decimal.Parse(GetValue(outputElements, "Insurance Esc Rate").Replace("%", ""));
            LiabilityRate = Decimal.Parse(GetValue(outputElements, "Liability Rate"));
            LiabilityRateEsc = Decimal.Parse(GetValue(outputElements, "Liability Rate Esc").Replace("%", ""));
            PropertyDamageCoverage = Decimal.Parse(GetValue(outputElements, "Property Damage Coverage"));
            AssetManagementInput = GetValues(outputElements, "Asset Management Input").ConvertAll(s => Decimal.Parse(s));
            AssetManagementEscRate = Decimal.Parse(GetValue(outputElements, "Asset Management Esc Rate"));
            SRECAssumptions = Decimal.Parse(GetValue(outputElements, "SREC Assumptions"));
            MaximumSolarSystemPremiumRatio = Decimal.Parse(GetValue(outputElements, "Maximum Solar System Premium Ratio"));
            InvestmentTaxCredit = Decimal.Parse(GetValue(outputElements, "Investment Tax Credit"));
            CorporateTaxRate = Decimal.Parse(GetValue(outputElements, "Corporate Tax Rate"));
            WACC = Decimal.Parse(GetValue(outputElements, "WACC"));
            NonAutoPayAmount = Decimal.Parse(GetValue(outputElements, "Non-Auto Pay Amount"));
            DownPayment = Decimal.Parse(GetValue(outputElements, "Down Payment"));
            SolarWaterHeaterBackup = GetValue(outputElements, "Solar Water Heater Backup");
            UsefulLife = int.Parse(GetValue(outputElements, "Useful Life"));
            BadDebtInput = Decimal.Parse(GetValue(outputElements, "Bad Debt Input"));
            EnergyValue = Decimal.Parse(GetValue(outputElements, "Energy Value"));
            ASPPerWGuess = Decimal.Parse(GetValue(outputElements, "ASP/W Guess"));
            MarginAmount = Decimal.Parse(GetValue(outputElements, "Margin Amount"));
            PPAAllowed = GetValue(outputElements, "PPA Allowed").Equals("Yes", StringComparison.InvariantCultureIgnoreCase);
            CustomerLeasePayments = GetValues(outputElements, "Customer Lease Payments").ConvertAll(s => Decimal.Parse(s));
            TerminationValues = GetValues(outputElements, "Termination Values").ConvertAll(s => Decimal.Parse(s));
            ConsumerLeaseFullPrepayment = Decimal.Parse(GetValue(outputElements, "Consumer Lease Full Prepayment"));
            DealerASPPerW = Decimal.Parse(GetValue(outputElements, "Dealer ASP/W"));
            DealerASP = Decimal.Parse(GetValue(outputElements, "Dealer ASP"));
            PISDate = DateTime.Parse(GetValue(outputElements, "PIS Date"));
            FirstLeasePaymentDate = DateTime.Parse(GetValue(outputElements, "First Lease Payment Date"));
            NetSolarCost  = Decimal.Parse(GetValue(outputElements, "Net Solar Cost "));
            SolarSystemPremiumRatio = Decimal.Parse(GetValue(outputElements, "Solar System Premium Ratio"));
            PremiumRatioTest = GetValue(outputElements, "Premium Ratio Test");
            MaxPPARate = Decimal.Parse(GetValue(outputElements, "Max PPA Rate"));
            TotalPaymentsYears1To20 = Decimal.Parse(GetValue(outputElements, "Total of Payments (Yr 1-20)"));
            TotalPaymentsYears21To30 = Decimal.Parse(GetValue(outputElements, "Total of Payments (Yr 21-30)"));
            TotalPaymentsYears1To30 = Decimal.Parse(GetValue(outputElements, "Total of Payments (Yr 1-30)"));
            EstimatedPricePerKWhAnnual = GetValues(outputElements, "Estimated Price/kWh (Annual)").ConvertAll(s => Decimal.Parse(s));
            EstimatedPricePerKWhLeaseTermWeightedAverageOver20years = GetValues(outputElements, "Estimated Price/kWh (Lease Term-weighted average over the 20 years)").ConvertAll(s => Decimal.Parse(s));
            YearNumbers = GetValues(outputElements, "Year Numbers").ConvertAll(s => int.Parse(s));
            PeriodicityOfPayments = GetValues(outputElements, "Periodicity of Payments");
            PPAPaymentsNoAutoPay = GetValues(outputElements, "PPA Payments (No Auto Pay)").ConvertAll(s => Decimal.Parse(s));
            SalesUseTaxesNoAutoPay = GetValues(outputElements, "Sales/Use Taxes (No Auto Pay)").ConvertAll(s => Decimal.Parse(s));
            TotalMonthlyPaymentsNoAutoPay = GetValues(outputElements, "Total Monthly Payments (No Auto Pay)").ConvertAll(s => Decimal.Parse(s));
            PPAPaymentsYesAutoPay = GetValues(outputElements, "PPA Payments (Yes Auto Pay)").ConvertAll(s => Decimal.Parse(s));
            SalesUseTaxesYesAutoPay = GetValues(outputElements, "Sales/Use Taxes (Yes Auto Pay)").ConvertAll(s => Decimal.Parse(s));
            TotalMonthlyPaymentsYesAutoPay = GetValues(outputElements, "Total Monthly Payments (Yes Auto Pay)").ConvertAll(s => Decimal.Parse(s));
            EstimatedTaxesOnDownPayment  = Decimal.Parse(GetValue(outputElements, "Estimated Taxes on Down Payment "));
            FirstYearMonthlyLeasePayment  = Decimal.Parse(GetValue(outputElements, "First Year Monthly Lease Payment "));
            FirstYearMonthlySalesUseTaxPayment = Decimal.Parse(GetValue(outputElements, "First Year Monthly Sales/Use Tax Payment"));
            TotalSalesUseTaxPaidNoAutoPay  = Decimal.Parse(GetValue(outputElements, "Total Sales/Use Tax Paid (No Auto Pay) "));
            TotalSalesUseTaxPaidYesAutoPay = Decimal.Parse(GetValue(outputElements, "Total Sales/Use Tax Paid (Yes Auto Pay)"));
            TotalLeasePaymentsNoAutoPay  = Decimal.Parse(GetValue(outputElements, "Total Lease Payments (No Auto Pay) "));
            TotalLeasePaymentsYesAutoPay  = Decimal.Parse(GetValue(outputElements, "Total Lease Payments (Yes Auto Pay) "));
            AnnualOutputSchedule = GetValues(outputElements, "Annual Output Schedule").ConvertAll(s => Decimal.Parse(s));
            TotalAdjustedCost = Decimal.Parse(GetValue(outputElements, "Total Adjusted Cost"));
            InitialAnnualRevenueYr1Amount = Decimal.Parse(GetValue(outputElements, "Initial Annual Revenue (Yr 1 Amount) "));
            ExtendedAnnualRevenueYr21Amount = Decimal.Parse(GetValue(outputElements, "Extended Annual Revenue (Yr 21 Amount) "));
            TotalRevenue = Decimal.Parse(GetValue(outputElements, "Total Revenue"));
            EffectiveAfterTaxYield = Decimal.Parse(GetValue(outputElements, "Effective After-Tax Yield"));
            AdjustedASPPerW = Decimal.Parse(GetValue(outputElements, "Adjusted ASP/W"));
            TotalRebate = Decimal.Parse(GetValue(outputElements, "Total Rebate"));
            CustomerUniqueID = GetValue(outputElements, "Customer unique ID");
            ProposalID = GetValue(outputElements, "Proposal ID");
            ModSolarCallVersionID = GetValue(outputElements, "ModSolar call version ID");
            UniqueFinancialRunIdentifier = GetValue(outputElements, "Unique financial run identifier");
            FinancialModelVersion = GetValue(outputElements, "Financial model version");
            Timestamp = GetValue(outputElements, "Timestamp");
        }

        private List<string> GetValues(IEnumerable<XElement> outputElements, string name)
        {
            var value = outputElements.Where(e => e.Attribute("name").Value == name).Select(a => a.Value).ToList();
            return value;
        }

        private string GetValue(IEnumerable<XElement> outputElements, string name)
        {
            var value = outputElements.Where(e => e.Attribute("name").Value == name).Select(a => a.Value).FirstOrDefault();
            return value;
        }
    }
}
