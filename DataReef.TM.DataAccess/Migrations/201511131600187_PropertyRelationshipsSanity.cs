namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PropertyRelationshipsSanity : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Properties", "IsArchive", c => c.Boolean(nullable: false));
            AlterColumn("dbo.Properties", "SortOrder", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Properties", "SortOrder", c => c.Int());
            AlterColumn("dbo.Properties", "IsArchive", c => c.Boolean());
        }
    }
}
