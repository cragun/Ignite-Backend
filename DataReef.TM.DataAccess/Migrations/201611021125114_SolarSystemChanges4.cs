namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SolarSystemChanges4 : DbMigration
    {
        public override void Up()
        {
            DropColumn("solar.Systems", "GenabilityTotalCost");
            DropColumn("solar.Systems", "GenabilityTotalConsumption");
            DropColumn("solar.Systems", "GenabilityTieredAverageUtilityCost");
            DropColumn("solar.Systems", "AnnualOutputDegradation");
            DropColumn("solar.Systems", "LoanPricePerWatt");
            DropColumn("solar.Systems", "LoanDownPayment");
            DropColumn("solar.Systems", "FederalInvestmentTaxCredit");
            DropColumn("solar.Systems", "LocalPBI");
            DropColumn("solar.Systems", "LocalPBITerm");
            DropColumn("solar.Systems", "LocalSREC");
            DropColumn("solar.Systems", "LocalSRECTerm");
            DropColumn("solar.Systems", "UpfrontRebate");
            DropColumn("solar.Systems", "PpaIsSelected");
            DropColumn("solar.Systems", "LoanIsSelected");
            DropColumn("solar.Systems", "Mosaic12YearLoanIsSelected");
            DropColumn("solar.Systems", "Mosaic20YearLoanIsSelected");
            DropColumn("solar.Systems", "GreenSky12YearLoanIsSelected");
            DropColumn("solar.Systems", "GreenSky20YearLoanIsSelected");
            DropColumn("solar.Systems", "LoanPricePerWattPricingOption");
            DropColumn("solar.Systems", "StateTaxIncentive");
            DropColumn("solar.Systems", "Spruce12YearLoanIsSelected");
            DropColumn("solar.Systems", "Spruce20YearLoanIsSelected");
            DropColumn("solar.Systems", "LoanExtraCosts");
            DropColumn("solar.Systems", "LoanTaxRate");
        }
        
        public override void Down()
        {
            AddColumn("solar.Systems", "LoanTaxRate", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("solar.Systems", "LoanExtraCosts", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("solar.Systems", "Spruce20YearLoanIsSelected", c => c.Boolean());
            AddColumn("solar.Systems", "Spruce12YearLoanIsSelected", c => c.Boolean());
            AddColumn("solar.Systems", "StateTaxIncentive", c => c.Double(nullable: false));
            AddColumn("solar.Systems", "LoanPricePerWattPricingOption", c => c.Int(nullable: false));
            AddColumn("solar.Systems", "GreenSky20YearLoanIsSelected", c => c.Boolean());
            AddColumn("solar.Systems", "GreenSky12YearLoanIsSelected", c => c.Boolean());
            AddColumn("solar.Systems", "Mosaic20YearLoanIsSelected", c => c.Boolean());
            AddColumn("solar.Systems", "Mosaic12YearLoanIsSelected", c => c.Boolean());
            AddColumn("solar.Systems", "LoanIsSelected", c => c.Boolean());
            AddColumn("solar.Systems", "PpaIsSelected", c => c.Boolean());
            AddColumn("solar.Systems", "UpfrontRebate", c => c.Double());
            AddColumn("solar.Systems", "LocalSRECTerm", c => c.Int());
            AddColumn("solar.Systems", "LocalSREC", c => c.Double());
            AddColumn("solar.Systems", "LocalPBITerm", c => c.Int());
            AddColumn("solar.Systems", "LocalPBI", c => c.Double());
            AddColumn("solar.Systems", "FederalInvestmentTaxCredit", c => c.Double());
            AddColumn("solar.Systems", "LoanDownPayment", c => c.Double());
            AddColumn("solar.Systems", "LoanPricePerWatt", c => c.Double());
            AddColumn("solar.Systems", "AnnualOutputDegradation", c => c.Single(nullable: false));
            AddColumn("solar.Systems", "GenabilityTieredAverageUtilityCost", c => c.Double(nullable: false));
            AddColumn("solar.Systems", "GenabilityTotalConsumption", c => c.Double(nullable: false));
            AddColumn("solar.Systems", "GenabilityTotalCost", c => c.Double(nullable: false));
        }
    }
}
