namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Adding_Description_Property_to_FinancePlanDefinition : DbMigration
    {
        public override void Up()
        {
            AddColumn("finance.PlanDefinitions", "Description", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("finance.PlanDefinitions", "Description");
        }
    }
}
