namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ProposalContractURLs : DbMigration
    {
        public override void Up()
        {
            AddColumn("solar.Proposals", "SignedProposalURL", c => c.String());
            AddColumn("solar.Proposals", "UnsignedContractURL", c => c.String());
            AddColumn("solar.Proposals", "SignedContractURL", c => c.String());
            AddColumn("solar.Proposals", "ContractExpiryDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("solar.Proposals", "ContractExpiryDate");
            DropColumn("solar.Proposals", "SignedContractURL");
            DropColumn("solar.Proposals", "UnsignedContractURL");
            DropColumn("solar.Proposals", "SignedProposalURL");
        }
    }
}
