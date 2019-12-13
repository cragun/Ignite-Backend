namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_IsManuallyEntered_to_PowerConsumption : DbMigration
    {
        public override void Up()
        {
            AddColumn("solar.PowerConsumption", "IsManuallyEntered", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("solar.PowerConsumption", "IsManuallyEntered");
        }
    }
}
