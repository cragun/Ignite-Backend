namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemovedusageCollectedfromProposalData : DbMigration
    {
        public override void Up()
        {
            DropColumn("solar.ProposalsData", "UsageCollected");
        }
        
        public override void Down()
        {
            AddColumn("solar.ProposalsData", "UsageCollected", c => c.Boolean(nullable: false));
        }
    }
}
