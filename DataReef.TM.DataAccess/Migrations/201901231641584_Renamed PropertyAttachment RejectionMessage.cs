namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RenamedPropertyAttachmentRejectionMessage : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PropertyAttachmentItems", "RejectionMessage", c => c.String());
            DropColumn("dbo.PropertyAttachmentItems", "RejectionMessagesJson");
        }
        
        public override void Down()
        {
            AddColumn("dbo.PropertyAttachmentItems", "RejectionMessagesJson", c => c.String());
            DropColumn("dbo.PropertyAttachmentItems", "RejectionMessage");
        }
    }
}
