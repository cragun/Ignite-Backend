namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LoanContractSign1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("solar.Proposals", "DateSigned", c => c.DateTime(nullable: false));
            DropColumn("solar.Proposals", "IsSigned");
            DropColumn("solar.Proposals", "Signature");
        }
        
        public override void Down()
        {
            AddColumn("solar.Proposals", "Signature", c => c.String());
            AddColumn("solar.Proposals", "IsSigned", c => c.Boolean(nullable: false));
            DropColumn("solar.Proposals", "DateSigned");
        }
    }
}
