namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_PropertyIDForeignKey_Migration : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.Notifications", "PropertyID");
            AddForeignKey("dbo.Notifications", "PropertyID", "dbo.Properties", "Guid");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Notifications", "PropertyID", "dbo.Properties");
            DropIndex("dbo.Notifications", new[] { "PropertyID" });
        }
    }
}
