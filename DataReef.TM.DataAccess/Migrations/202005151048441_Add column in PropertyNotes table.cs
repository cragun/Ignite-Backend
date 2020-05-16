namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddcolumninPropertyNotestable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PropertyNotes", "ParentID", c => c.Guid(nullable: false));
            AddColumn("dbo.PropertyNotes", "ContentType", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.PropertyNotes", "ContentType");
            DropColumn("dbo.PropertyNotes", "ParentID");
        }
    }
}
