namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Removed_unused_properties_on_panels_and_inverters : DbMigration
    {
        public override void Up()
        {
            DropColumn("solar.Inverters", "OUID");
            DropColumn("solar.Inverters", "Rank");
            DropColumn("solar.Inverters", "MinSystemSizeInWatts");
            DropColumn("solar.Panels", "IsActive");
            DropColumn("solar.Panels", "OUID");
            DropColumn("solar.Panels", "Rank");
        }
        
        public override void Down()
        {
            AddColumn("solar.Panels", "Rank", c => c.Int(nullable: false));
            AddColumn("solar.Panels", "OUID", c => c.Guid(nullable: false));
            AddColumn("solar.Panels", "IsActive", c => c.Boolean(nullable: false));
            AddColumn("solar.Inverters", "MinSystemSizeInWatts", c => c.Int(nullable: false));
            AddColumn("solar.Inverters", "Rank", c => c.Int(nullable: false));
            AddColumn("solar.Inverters", "OUID", c => c.Guid(nullable: false));
        }
    }
}
