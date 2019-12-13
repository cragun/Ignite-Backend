namespace DataReef.TM.DataAccess.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class AddedFieldsToSolarSystem : DbMigration
    {
        public override void Up()
        {
            AddColumn("solar.Systems", "LoanPricePerWatt", c => c.Double());
            AddColumn("solar.Systems", "LoanDownPayment", c => c.Double());
            AddColumn("solar.Systems", "FederalInvestmentTaxCredit", c => c.Double());
            AddColumn("solar.Systems", "LocalPBI", c => c.Double());
            AddColumn("solar.Systems", "LocalPBITerm", c => c.Int());
            AddColumn("solar.Systems", "LocalSREC", c => c.Double());
            AddColumn("solar.Systems", "LocalSRECTerm", c => c.Int());
            AddColumn("solar.Systems", "UpfrontRebate", c => c.Double());
            AddColumn("solar.Systems", "PpaIsSelected", c => c.Boolean());
            AddColumn("solar.Systems", "LoanIsSelected", c => c.Boolean());
            AddColumn("solar.Systems", "Mosaic12YearLoanIsSelected", c => c.Boolean());
            AddColumn("solar.Systems", "Mosaic20YearLoanIsSelected", c => c.Boolean());
            AddColumn("solar.Systems", "GreenSky12YearLoanIsSelected", c => c.Boolean());
            AddColumn("solar.Systems", "GreenSky20YearLoanIsSelected", c => c.Boolean());
        }
        
        public override void Down()
        {
            DropColumn("solar.Systems", "GreenSky20YearLoanIsSelected");
            DropColumn("solar.Systems", "GreenSky12YearLoanIsSelected");
            DropColumn("solar.Systems", "Mosaic20YearLoanIsSelected");
            DropColumn("solar.Systems", "Mosaic12YearLoanIsSelected");
            DropColumn("solar.Systems", "LoanIsSelected");
            DropColumn("solar.Systems", "PpaIsSelected");
            DropColumn("solar.Systems", "UpfrontRebate");
            DropColumn("solar.Systems", "LocalSRECTerm");
            DropColumn("solar.Systems", "LocalSREC");
            DropColumn("solar.Systems", "LocalPBITerm");
            DropColumn("solar.Systems", "LocalPBI");
            DropColumn("solar.Systems", "FederalInvestmentTaxCredit");
            DropColumn("solar.Systems", "LoanDownPayment");
            DropColumn("solar.Systems", "LoanPricePerWatt");
        }
    }
}
