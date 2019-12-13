namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ProposalDocumentRename : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "solar.ProposalDocuments", newName: "FinanceDocuments");
        }
        
        public override void Down()
        {
            RenameTable(name: "solar.FinanceDocuments", newName: "ProposalDocuments");
        }
    }
}
