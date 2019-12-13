namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MovedZoomLevelfrompropertytoproposal : DbMigration
    {
        public override void Up()
        {
            AddColumn("solar.Proposals", "ZoomLevel", c => c.Decimal(precision: 18, scale: 2));
            DropColumn("dbo.Properties", "ZoomLevel");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Properties", "ZoomLevel", c => c.Int());
            DropColumn("solar.Proposals", "ZoomLevel");
        }
    }
}
