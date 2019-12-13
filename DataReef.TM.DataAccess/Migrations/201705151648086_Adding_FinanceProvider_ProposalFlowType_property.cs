namespace DataReef.TM.DataAccess.Migrations
{
    using DataReef.TM.Models.Enums;
    using System;
    using System.Data.Entity.Migrations;

    public partial class Adding_FinanceProvider_ProposalFlowType_property : DbMigration
    {
        public override void Up()
        {
            AddColumn("finance.Providers", "ProposalFlowType", c => c.Int(nullable: false, defaultValue: (int)FinanceProviderProposalFlowType.None));
        }

        public override void Down()
        {
            DropColumn("finance.Providers", "ProposalFlowType");
        }
    }
}
