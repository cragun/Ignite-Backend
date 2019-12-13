namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ProposalData_RemoveJsons : DbMigration
    {
        public override void Up()
        {
            DropColumn("solar.ProposalsData", "UserInputDataJSON");
            DropColumn("solar.ProposalsData", "DocumentDataJSON");
        }
        
        public override void Down()
        {
            AddColumn("solar.ProposalsData", "DocumentDataJSON", c => c.String());
            AddColumn("solar.ProposalsData", "UserInputDataJSON", c => c.String());
        }
    }
}
