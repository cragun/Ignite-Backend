namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddingIndicesonNameandAddress1 : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.Properties", "Name");
            CreateIndex("dbo.Properties", "Address1");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Properties", new[] { "Address1" });
            DropIndex("dbo.Properties", new[] { "Name" });
        }
    }
}
