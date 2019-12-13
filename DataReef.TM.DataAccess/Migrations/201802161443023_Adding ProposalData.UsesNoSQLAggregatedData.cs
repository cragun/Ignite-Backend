namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class AddingProposalDataUsesNoSQLAggregatedData : DbMigration
    {
        public override void Up()
        {
            AddColumn("solar.ProposalsData", "UsesNoSQLAggregatedData", c => c.Boolean(nullable: false));
        }

        public override void Down()
        {
            DropColumn("solar.ProposalsData", "UsesNoSQLAggregatedData");
        }
    }
}
