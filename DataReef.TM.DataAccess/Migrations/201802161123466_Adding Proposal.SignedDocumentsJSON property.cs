namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class AddingProposalSignedDocumentsJSONproperty : DbMigration
    {
        public override void Up()
        {
            AddColumn("solar.Proposals", "SignedDocumentsJSON", c => c.String());
        }

        public override void Down()
        {
            DropColumn("solar.Proposals", "SignedDocumentsJSON");
        }
    }
}
