namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SolarSystemChanges3 : DbMigration
    {
        public override void Up()
        {
            DropColumn("solar.Systems", "LoanRateType");
            DropColumn("solar.Systems", "LoanDaysDeferred");
        }
        
        public override void Down()
        {
            AddColumn("solar.Systems", "LoanDaysDeferred", c => c.Int(nullable: false));
            AddColumn("solar.Systems", "LoanRateType", c => c.String());
        }
    }
}
