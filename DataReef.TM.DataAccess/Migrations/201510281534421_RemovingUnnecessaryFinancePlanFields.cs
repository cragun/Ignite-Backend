namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemovingUnnecessaryFinancePlanFields : DbMigration
    {
        public override void Up()
        {
            DropColumn("solar.FinancePlans", "Description");
            DropColumn("solar.FinancePlans", "CurrentUtilityCost");
            DropColumn("solar.FinancePlans", "PricePerWatt");
            DropColumn("solar.FinancePlans", "SalesTax");
            DropColumn("solar.FinancePlans", "FederalTaxCredit");
            DropColumn("solar.FinancePlans", "StateLocalTaxCredit");
            DropColumn("solar.FinancePlans", "LocalRebate");
            DropColumn("solar.FinancePlans", "LocalPBI");
            DropColumn("solar.FinancePlans", "LocalPBIMonths");
            DropColumn("solar.FinancePlans", "LocalPBIAnnualIncrease");
            DropColumn("solar.FinancePlans", "DownPayment");
            DropColumn("solar.FinancePlans", "PostSolarUtilityCost");
            DropColumn("solar.FinancePlans", "Term1Months");
            DropColumn("solar.FinancePlans", "Term1InterestRate");
            DropColumn("solar.FinancePlans", "Term1Payment");
            DropColumn("solar.FinancePlans", "Term2Months");
            DropColumn("solar.FinancePlans", "Term2InterestRate");
            DropColumn("solar.FinancePlans", "Term2Payment");
            DropColumn("solar.FinancePlans", "BuyDownMonth");
            DropColumn("solar.FinancePlans", "BuyDownAmount");
            DropColumn("solar.FinancePlans", "LoanAmount");
            DropColumn("solar.FinancePlans", "OriginationFee");
            DropColumn("solar.FinancePlans", "DocumentFee");
            DropColumn("solar.FinancePlans", "EffectiveAPR");
            DropColumn("solar.FinancePlans", "MonthlyPayments");
            DropColumn("solar.FinancePlans", "AnnualPBISchedule");
            DropColumn("solar.FinancePlans", "AnnualSRECSchedule");
            DropColumn("solar.FinancePlans", "SolarSystemLoanPayment");
            DropColumn("solar.FinancePlans", "NetSolarCosts");
            DropColumn("solar.FinancePlans", "SolarSystemPremiumRatio");
            DropColumn("solar.FinancePlans", "MaxSolarSystemPremiumRatio");
            DropColumn("solar.FinancePlans", "PremiumRatioTestResults");
            DropColumn("solar.FinancePlans", "MaxASP");
            DropColumn("solar.FinancePlans", "MaxASPPerWatt");
            DropColumn("solar.FinancePlans", "MaxASPFederalITC");
            DropColumn("solar.FinancePlans", "PaymentEscalation");
            DropColumn("solar.FinancePlans", "FinancialModelVersion");
            DropColumn("solar.FinancePlans", "FinancialRunIdentifier");
        }
        
        public override void Down()
        {
            AddColumn("solar.FinancePlans", "FinancialRunIdentifier", c => c.String(maxLength: 50));
            AddColumn("solar.FinancePlans", "FinancialModelVersion", c => c.String(maxLength: 50));
            AddColumn("solar.FinancePlans", "PaymentEscalation", c => c.Single(nullable: false));
            AddColumn("solar.FinancePlans", "MaxASPFederalITC", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("solar.FinancePlans", "MaxASPPerWatt", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("solar.FinancePlans", "MaxASP", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("solar.FinancePlans", "PremiumRatioTestResults", c => c.String(maxLength: 50));
            AddColumn("solar.FinancePlans", "MaxSolarSystemPremiumRatio", c => c.Single(nullable: false));
            AddColumn("solar.FinancePlans", "SolarSystemPremiumRatio", c => c.Single(nullable: false));
            AddColumn("solar.FinancePlans", "NetSolarCosts", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("solar.FinancePlans", "SolarSystemLoanPayment", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("solar.FinancePlans", "AnnualSRECSchedule", c => c.String());
            AddColumn("solar.FinancePlans", "AnnualPBISchedule", c => c.String());
            AddColumn("solar.FinancePlans", "MonthlyPayments", c => c.String());
            AddColumn("solar.FinancePlans", "EffectiveAPR", c => c.Single(nullable: false));
            AddColumn("solar.FinancePlans", "DocumentFee", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("solar.FinancePlans", "OriginationFee", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("solar.FinancePlans", "LoanAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("solar.FinancePlans", "BuyDownAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("solar.FinancePlans", "BuyDownMonth", c => c.Int(nullable: false));
            AddColumn("solar.FinancePlans", "Term2Payment", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("solar.FinancePlans", "Term2InterestRate", c => c.Single(nullable: false));
            AddColumn("solar.FinancePlans", "Term2Months", c => c.Int(nullable: false));
            AddColumn("solar.FinancePlans", "Term1Payment", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("solar.FinancePlans", "Term1InterestRate", c => c.Single(nullable: false));
            AddColumn("solar.FinancePlans", "Term1Months", c => c.Int(nullable: false));
            AddColumn("solar.FinancePlans", "PostSolarUtilityCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("solar.FinancePlans", "DownPayment", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("solar.FinancePlans", "LocalPBIAnnualIncrease", c => c.Single(nullable: false));
            AddColumn("solar.FinancePlans", "LocalPBIMonths", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("solar.FinancePlans", "LocalPBI", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("solar.FinancePlans", "LocalRebate", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("solar.FinancePlans", "StateLocalTaxCredit", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("solar.FinancePlans", "FederalTaxCredit", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("solar.FinancePlans", "SalesTax", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("solar.FinancePlans", "PricePerWatt", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("solar.FinancePlans", "CurrentUtilityCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("solar.FinancePlans", "Description", c => c.String());
        }
    }
}
