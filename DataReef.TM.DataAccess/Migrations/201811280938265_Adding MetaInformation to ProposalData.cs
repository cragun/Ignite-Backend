namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddingMetaInformationtoProposalData : DbMigration
    {
        public override void Up()
        {
            AddColumn("solar.ProposalsData", "MetaInformationJSON", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("solar.ProposalsData", "MetaInformationJSON");
        }
    }
}
