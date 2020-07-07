namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddexcludeProposalJSON : DbMigration
    {
        public override void Up()
        {
            AddColumn("solar.ProposalsData", "excludeProposalJSON", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("solar.ProposalsData", "excludeProposalJSON");
        }
    }
}
