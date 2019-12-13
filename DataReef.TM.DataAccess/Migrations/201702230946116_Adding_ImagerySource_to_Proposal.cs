namespace DataReef.TM.DataAccess.Migrations
{
    using DataReef.TM.Models.Enums;
    using System;
    using System.Data.Entity.Migrations;

    public partial class Adding_ImagerySource_to_Proposal : DbMigration
    {
        public override void Up()
        {
            AddColumn("solar.Proposals", "ImagerySource", c => c.Int(nullable: false, defaultValue: (int)ProposalImagerySource.GoogleMaps));
        }

        public override void Down()
        {
            DropColumn("solar.Proposals", "ImagerySource");
        }
    }
}
