namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Adding_ProposalID_to_ProposalData_entity : DbMigration
    {
        public override void Up()
        {
            AddColumn("solar.ProposalsData", "ProposalID", c => c.Guid(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("solar.ProposalsData", "ProposalID");
        }
    }
}
