namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class migration : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OUs", "IsTerritoryAdd", c => c.Boolean(nullable: false));
            AddColumn("solar.FinancePlans", "SunlightReqJson", c => c.String());
            AddColumn("solar.FinancePlans", "SunlightResponseJson", c => c.String());
            AddColumn("solar.ProposalsData", "excludeProposalJSON", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("solar.ProposalsData", "excludeProposalJSON");
            DropColumn("solar.FinancePlans", "SunlightResponseJson");
            DropColumn("solar.FinancePlans", "SunlightReqJson");
            DropColumn("dbo.OUs", "IsTerritoryAdd");
        }
    }
}
