namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddingAudittoPropertyAttachment : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PropertyAttachments", "AuditJSON", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.PropertyAttachments", "AuditJSON");
        }
    }
}
