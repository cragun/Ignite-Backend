namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FinanceDocumentErrorMessage : DbMigration
    {
        public override void Up()
        {
            AddColumn("solar.FinanceDocuments", "ErrorMessage", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("solar.FinanceDocuments", "ErrorMessage");
        }
    }
}
