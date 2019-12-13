namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Remove_contract_properties_from_Proposal : DbMigration
    {
        public override void Up()
        {
            DropColumn("solar.Proposals", "DateSigned");
            DropColumn("solar.Proposals", "SignedProposalURL");
            DropColumn("solar.Proposals", "UnsignedContractURL");
            DropColumn("solar.Proposals", "SignedContractURL");
            DropColumn("solar.Proposals", "ContractExpiryDate");
        }
        
        public override void Down()
        {
            AddColumn("solar.Proposals", "ContractExpiryDate", c => c.DateTime());
            AddColumn("solar.Proposals", "SignedContractURL", c => c.String());
            AddColumn("solar.Proposals", "UnsignedContractURL", c => c.String());
            AddColumn("solar.Proposals", "SignedProposalURL", c => c.String());
            AddColumn("solar.Proposals", "DateSigned", c => c.DateTime());
        }
    }
}
