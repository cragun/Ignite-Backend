namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Proposal_Add_ProposalURL : DbMigration
    {
        public override void Up()
        {
            AddColumn("solar.Proposals", "ProposalURL", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("solar.Proposals", "ProposalURL");
        }
    }
}
