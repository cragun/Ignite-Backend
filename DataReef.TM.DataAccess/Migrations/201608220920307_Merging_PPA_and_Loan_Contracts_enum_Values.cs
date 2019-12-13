namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class Merging_PPA_and_Loan_Contracts_enum_Values : DbMigration
    {
        public override void Up()
        {
            // PPAContract(3) and LoanContract(4) will be merged into Contract(3)
            Sql("UPDATE solar.ProposalDocuments SET DocumentType = 3 WHERE DocumentType = 4");
        }

        public override void Down()
        {
        }
    }
}
