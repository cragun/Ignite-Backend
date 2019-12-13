namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SolarSystemChanges2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("solar.Systems", "StateTaxIncentive", c => c.Double(nullable: false));
            DropColumn("solar.Systems", "LoanPrepaymentPercentage");
            DropColumn("solar.Systems", "LoanPrepaymentAmount");
        }
        
        public override void Down()
        {
            AddColumn("solar.Systems", "LoanPrepaymentAmount", c => c.Double(nullable: false));
            AddColumn("solar.Systems", "LoanPrepaymentPercentage", c => c.Double(nullable: false));
            DropColumn("solar.Systems", "StateTaxIncentive");
        }
    }
}
