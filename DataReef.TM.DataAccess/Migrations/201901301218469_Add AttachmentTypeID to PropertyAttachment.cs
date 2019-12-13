namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAttachmentTypeIDtoPropertyAttachment : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PropertyAttachments", "AttachmentTypeID", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.PropertyAttachments", "AttachmentTypeID");
        }
    }
}
