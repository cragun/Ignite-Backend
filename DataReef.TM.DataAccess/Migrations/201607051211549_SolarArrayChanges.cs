namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SolarArrayChanges : DbMigration
    {
        public override void Up()
        {
            AddColumn("solar.Arrays", "FireOffsetIsEnabled", c => c.Boolean(nullable: false));
            AddColumn("solar.Arrays", "FireOffset", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("solar.Arrays", "FireOffset");
            DropColumn("solar.Arrays", "FireOffsetIsEnabled");
        }
    }
}
