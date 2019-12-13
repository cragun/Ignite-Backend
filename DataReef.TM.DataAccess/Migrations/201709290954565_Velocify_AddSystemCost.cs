namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Velocify_AddSystemCost : DbMigration
    {
        public override void Up()
        {
            AddColumn("PRMI.VelocifyRequests", "SystemCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("PRMI.VelocifyRequests", "OriginalMortgageStartDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("PRMI.VelocifyRequests", "OriginalMortgageStartDate", c => c.DateTime(nullable: false));
            DropColumn("PRMI.VelocifyRequests", "SystemCost");
        }
    }
}
