namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Misc_Pending_Changes : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.WebHooks", "OUID");
            RenameColumn(table: "dbo.WebHooks", name: "OU_Guid", newName: "OUID");
            RenameIndex(table: "dbo.WebHooks", name: "IX_OU_Guid", newName: "IX_OUID");
          //  DropColumn("dbo.Territories", "PropertyCount");
        }
        
        public override void Down()
        {
        //    AddColumn("dbo.Territories", "PropertyCount", c => c.Int(nullable: false));
            RenameIndex(table: "dbo.WebHooks", name: "IX_OUID", newName: "IX_OU_Guid");
            RenameColumn(table: "dbo.WebHooks", name: "OUID", newName: "OU_Guid");
            AddColumn("dbo.WebHooks", "OUID", c => c.Guid(nullable: false));
        }
    }
}
