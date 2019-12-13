namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SolarSystemChanges1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("solar.Systems", "LoanRateType", c => c.String());
            AddColumn("solar.Systems", "LoanDaysDeferred", c => c.Int(nullable: false));
            AddColumn("solar.Systems", "LoanPrepaymentPercentage", c => c.Double(nullable: false));
            AddColumn("solar.Systems", "LoanPrepaymentAmount", c => c.Double(nullable: false));
            AddColumn("solar.Systems", "Spruce12YearLoanIsSelected", c => c.Boolean(nullable: false));
            AddColumn("solar.Systems", "Spruce20YearLoanIsSelected", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("solar.Systems", "Spruce20YearLoanIsSelected");
            DropColumn("solar.Systems", "Spruce12YearLoanIsSelected");
            DropColumn("solar.Systems", "LoanPrepaymentAmount");
            DropColumn("solar.Systems", "LoanPrepaymentPercentage");
            DropColumn("solar.Systems", "LoanDaysDeferred");
            DropColumn("solar.Systems", "LoanRateType");
        }
    }
}
