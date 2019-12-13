namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class RemovingProposalDatanavproperties : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("solar.ProposalsData", "FinancePlanID", "solar.FinancePlans");
            DropIndex("solar.ProposalsData", new[] { "FinancePlanID" });
        }

        public override void Down()
        {
            CreateIndex("solar.ProposalsData", "FinancePlanID");
            AddForeignKey("solar.ProposalsData", "FinancePlanID", "solar.FinancePlans", "Guid");
        }
    }
}
