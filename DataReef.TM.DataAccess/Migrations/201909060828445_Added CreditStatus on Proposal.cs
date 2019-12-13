namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedCreditStatusonProposal : DbMigration
    {
        public override void Up()
        {
            AddColumn("solar.Proposals", "CreditStatus", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("solar.Proposals", "CreditStatus");
        }
    }
}
