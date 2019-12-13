namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatestoNotes : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Notifications", "Status", c => c.Int(nullable: false));
            AlterColumn("dbo.Notifications", "Value", c => c.Guid());
            CreateIndex("dbo.Notifications", "Status");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Notifications", new[] { "Status" });
            AlterColumn("dbo.Notifications", "Value", c => c.String());
            DropColumn("dbo.Notifications", "Status");
        }
    }
}
