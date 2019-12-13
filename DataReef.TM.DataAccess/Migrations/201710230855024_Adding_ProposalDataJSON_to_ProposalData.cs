namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Adding_ProposalDataJSON_to_ProposalData : DbMigration
    {
        public override void Up()
        {
            AddColumn("solar.ProposalsData", "ProposalDataJSON", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("solar.ProposalsData", "ProposalDataJSON");
        }
    }
}
