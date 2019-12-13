namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LogAuthentications : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Authentications",
                c => new
                    {
                        Guid = c.Guid(nullable: false),
                        UserID = c.Guid(nullable: false),
                        DeviceID = c.Guid(nullable: false),
                        DateAuthenticated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Guid)
                .Index(t => t.UserID);
            
            AddColumn("dbo.Users", "IsNonBillable", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropIndex("dbo.Authentications", new[] { "UserID" });
            DropColumn("dbo.Users", "IsNonBillable");
            DropTable("dbo.Authentications");
        }
    }
}
