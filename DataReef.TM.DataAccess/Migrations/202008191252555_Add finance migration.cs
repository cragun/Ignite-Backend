namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Addfinancemigration : DbMigration
    {
        public override void Up()
        {
            //AddColumn("finance.PlanDefinitions", "LenderFee", c => c.Double());
            //AddColumn("finance.PlanDefinitions", "PPW", c => c.Double());
        }
        
        public override void Down()
        {
            //DropColumn("finance.PlanDefinitions", "PPW");
            //DropColumn("finance.PlanDefinitions", "LenderFee");
        }
    }
}
