namespace DataReef.TM.DataAccess.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class SolarProposalChanges : DbMigration
    {
        public override void Up()
        {
            AlterColumn("solar.Proposals", "PropertyID", c => c.Guid(nullable: false));
            CreateIndex("solar.Proposals", "PropertyID");
            AddForeignKey("solar.Proposals", "PropertyID", "dbo.Properties", "Guid");
        }
        
        public override void Down()
        {
            DropForeignKey("solar.Proposals", "PropertyID", "dbo.Properties");
            DropIndex("solar.Proposals", new[] { "PropertyID" });
            AlterColumn("solar.Proposals", "PropertyID", c => c.Guid());
        }
    }
}
