namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class AddingMediaItemTypetoProposalMediaItem : DbMigration
    {
        public override void Up()
        {
            AddColumn("solar.ProposalMediaItems", "MediaItemType", c => c.Int(nullable: false));
        }

        public override void Down()
        {
            DropColumn("solar.ProposalMediaItems", "MediaItemType");
        }
    }
}
