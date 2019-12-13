namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LoanContractSign : DbMigration
    {
        public override void Up()
        {
            AddColumn("solar.Proposals", "ContractID", c => c.String(maxLength: 22));
            AddColumn("solar.Proposals", "IsSigned", c => c.Boolean(nullable: false));
            CreateIndex("solar.Proposals", "ContractID");
        }
        
        public override void Down()
        {
            DropIndex("solar.Proposals", new[] { "ContractID" });
            DropColumn("solar.Proposals", "IsSigned");
            DropColumn("solar.Proposals", "ContractID");
        }
    }
}
