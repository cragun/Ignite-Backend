namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedZoomLeveltoProperty : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Properties", "ZoomLevel", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Properties", "ZoomLevel");
        }
    }
}
