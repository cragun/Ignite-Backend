namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class HardCreditCheck1 : DbMigration
    {
        public override void Up()
        {
            DropIndex("Spruce.QuoteRequests", new[] { "AppEmploymentID" });
            DropIndex("Spruce.QuoteRequests", new[] { "CoAppEmploymentID" });
            DropIndex("Spruce.QuoteRequests", new[] { "AppIncomeDebtID" });
            DropIndex("Spruce.QuoteRequests", new[] { "CoAppIncomeDebtID" });
            RenameColumn(table: "Spruce.QuoteRequests", name: "AppEmploymentID", newName: "AppEmployment_Guid");
            RenameColumn(table: "Spruce.QuoteRequests", name: "AppIncomeDebtID", newName: "AppIncomeDebt_Guid");
            RenameColumn(table: "Spruce.QuoteRequests", name: "CoAppEmploymentID", newName: "CoAppEmployment_Guid");
            RenameColumn(table: "Spruce.QuoteRequests", name: "CoAppIncomeDebtID", newName: "CoAppIncomeDebt_Guid");
            AddColumn("Spruce.Employments", "AppQuoteId", c => c.Guid());
            AddColumn("Spruce.Employments", "CoAppQuoteId", c => c.Guid());
            AddColumn("Spruce.IncomeDebts", "AppQuoteId", c => c.Guid());
            AddColumn("Spruce.IncomeDebts", "CoAppQuoteId", c => c.Guid());
            AlterColumn("Spruce.QuoteRequests", "AppEmployment_Guid", c => c.Guid());
            AlterColumn("Spruce.QuoteRequests", "CoAppEmployment_Guid", c => c.Guid());
            AlterColumn("Spruce.QuoteRequests", "AppIncomeDebt_Guid", c => c.Guid());
            AlterColumn("Spruce.QuoteRequests", "CoAppIncomeDebt_Guid", c => c.Guid());
            CreateIndex("Spruce.QuoteRequests", "AppEmployment_Guid");
            CreateIndex("Spruce.QuoteRequests", "AppIncomeDebt_Guid");
            CreateIndex("Spruce.QuoteRequests", "CoAppEmployment_Guid");
            CreateIndex("Spruce.QuoteRequests", "CoAppIncomeDebt_Guid");
            CreateIndex("Spruce.Employments", "AppQuoteId");
            CreateIndex("Spruce.Employments", "CoAppQuoteId");
            CreateIndex("Spruce.IncomeDebts", "AppQuoteId");
            CreateIndex("Spruce.IncomeDebts", "CoAppQuoteId");
            AddForeignKey("Spruce.Employments", "AppQuoteId", "Spruce.QuoteRequests", "Guid");
            AddForeignKey("Spruce.Employments", "CoAppQuoteId", "Spruce.QuoteRequests", "Guid");
            AddForeignKey("Spruce.IncomeDebts", "AppQuoteId", "Spruce.QuoteRequests", "Guid");
            AddForeignKey("Spruce.IncomeDebts", "CoAppQuoteId", "Spruce.QuoteRequests", "Guid");
        }
        
        public override void Down()
        {
            DropForeignKey("Spruce.IncomeDebts", "CoAppQuoteId", "Spruce.QuoteRequests");
            DropForeignKey("Spruce.IncomeDebts", "AppQuoteId", "Spruce.QuoteRequests");
            DropForeignKey("Spruce.Employments", "CoAppQuoteId", "Spruce.QuoteRequests");
            DropForeignKey("Spruce.Employments", "AppQuoteId", "Spruce.QuoteRequests");
            DropIndex("Spruce.IncomeDebts", new[] { "CoAppQuoteId" });
            DropIndex("Spruce.IncomeDebts", new[] { "AppQuoteId" });
            DropIndex("Spruce.Employments", new[] { "CoAppQuoteId" });
            DropIndex("Spruce.Employments", new[] { "AppQuoteId" });
            DropIndex("Spruce.QuoteRequests", new[] { "CoAppIncomeDebt_Guid" });
            DropIndex("Spruce.QuoteRequests", new[] { "CoAppEmployment_Guid" });
            DropIndex("Spruce.QuoteRequests", new[] { "AppIncomeDebt_Guid" });
            DropIndex("Spruce.QuoteRequests", new[] { "AppEmployment_Guid" });
            AlterColumn("Spruce.QuoteRequests", "CoAppIncomeDebt_Guid", c => c.Guid(nullable: false));
            AlterColumn("Spruce.QuoteRequests", "AppIncomeDebt_Guid", c => c.Guid(nullable: false));
            AlterColumn("Spruce.QuoteRequests", "CoAppEmployment_Guid", c => c.Guid(nullable: false));
            AlterColumn("Spruce.QuoteRequests", "AppEmployment_Guid", c => c.Guid(nullable: false));
            DropColumn("Spruce.IncomeDebts", "CoAppQuoteId");
            DropColumn("Spruce.IncomeDebts", "AppQuoteId");
            DropColumn("Spruce.Employments", "CoAppQuoteId");
            DropColumn("Spruce.Employments", "AppQuoteId");
            RenameColumn(table: "Spruce.QuoteRequests", name: "CoAppIncomeDebt_Guid", newName: "CoAppIncomeDebtID");
            RenameColumn(table: "Spruce.QuoteRequests", name: "CoAppEmployment_Guid", newName: "CoAppEmploymentID");
            RenameColumn(table: "Spruce.QuoteRequests", name: "AppIncomeDebt_Guid", newName: "AppIncomeDebtID");
            RenameColumn(table: "Spruce.QuoteRequests", name: "AppEmployment_Guid", newName: "AppEmploymentID");
            CreateIndex("Spruce.QuoteRequests", "CoAppIncomeDebtID");
            CreateIndex("Spruce.QuoteRequests", "AppIncomeDebtID");
            CreateIndex("Spruce.QuoteRequests", "CoAppEmploymentID");
            CreateIndex("Spruce.QuoteRequests", "AppEmploymentID");
        }
    }
}
