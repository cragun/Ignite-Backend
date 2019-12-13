namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class GridOffsetandSystemJson : DbMigration
    {
        public override void Up()
        {
            AddColumn("solar.Systems", "GridRotation", c => c.Single(nullable: false));
            AddColumn("solar.Systems", "GridOffsetX", c => c.Int(nullable: false));
            AddColumn("solar.Systems", "GridOffsetY", c => c.Int(nullable: false));
            AddColumn("solar.Systems", "SystemJSON", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("solar.Systems", "SystemJSON");
            DropColumn("solar.Systems", "GridOffsetY");
            DropColumn("solar.Systems", "GridOffsetX");
            DropColumn("solar.Systems", "GridRotation");
        }
    }
}
