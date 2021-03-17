namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Addattachmentsfieldinpropertynotes : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PropertyNotes", "Attachments", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.PropertyNotes", "Attachments");
        }
    }
}
