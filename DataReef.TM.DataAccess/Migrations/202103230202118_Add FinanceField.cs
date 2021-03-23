namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddFinanceField : DbMigration
    {
        public override void Up()
        {
            AddColumn("finance.PlanDefinitions", "TermExternalID", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("finance.PlanDefinitions", "TermExternalID");
        }
    }
}
