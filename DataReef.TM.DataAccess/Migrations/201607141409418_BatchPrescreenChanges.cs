namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BatchPrescreenChanges : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PrescreenDetails", "PropertyID", c => c.Guid(nullable: false));
            AlterColumn("dbo.PrescreenDetails", "AddressID", c => c.Guid());
            CreateIndex("dbo.PrescreenDetails", "PropertyID");
            AddForeignKey("dbo.PrescreenDetails", "PropertyID", "dbo.Properties", "Guid");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PrescreenDetails", "PropertyID", "dbo.Properties");
            DropIndex("dbo.PrescreenDetails", new[] { "PropertyID" });
            AlterColumn("dbo.PrescreenDetails", "AddressID", c => c.Guid(nullable: false));
            DropColumn("dbo.PrescreenDetails", "PropertyID");
        }
    }
}
