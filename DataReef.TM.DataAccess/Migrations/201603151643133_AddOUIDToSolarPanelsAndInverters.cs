namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddOUIDToSolarPanelsAndInverters : DbMigration
    {
        public override void Up()
        {
            AddColumn("solar.Inverters", "OUID", c => c.Guid(nullable: false));
            AddColumn("solar.Panels", "OUID", c => c.Guid(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("solar.Panels", "OUID");
            DropColumn("solar.Inverters", "OUID");
        }
    }
}
