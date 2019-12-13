namespace DataReef.TM.DataAccess.Migrations
{
    using DataReef.TM.Models.Enums;
    using System;
    using System.Data.Entity.Migrations;

    public partial class Adding_DesignSystemType_to_Proposal_Model : DbMigration
    {
        public override void Up()
        {
            AddColumn("solar.Proposals", "DesignSystemType", c => c.Int(nullable: false, defaultValue: (int)ProposalDesignSystemType.AdvancedDrawing));
        }

        public override void Down()
        {
            DropColumn("solar.Proposals", "DesignSystemType");
        }
    }
}
