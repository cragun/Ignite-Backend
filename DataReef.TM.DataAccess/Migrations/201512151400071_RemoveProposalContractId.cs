namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveProposalContractId : DbMigration
    {
        public override void Up()
        {
            DropIndex("solar.Proposals", new[] { "ContractID" });
            DropColumn("solar.Proposals", "ContractID");
        }
        
        public override void Down()
        {
            AddColumn("solar.Proposals", "ContractID", c => c.String(maxLength: 22));
            CreateIndex("solar.Proposals", "ContractID");
        }
    }
}
