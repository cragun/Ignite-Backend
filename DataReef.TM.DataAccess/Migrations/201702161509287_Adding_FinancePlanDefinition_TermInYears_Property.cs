namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class Adding_FinancePlanDefinition_TermInYears_Property : DbMigration
    {
        public override void Up()
        {
            AddColumn("finance.PlanDefinitions", "TermInYears", c => c.Int(nullable: false));
            // setting the TermInYears value, by getting the 2nd word from the Name (e.g. for "Spruce 20 / 4.99%" will get "20")
            Sql(@"UPDATE finance.PlanDefinitions
                  SET TermInYears = CASE WHEN TRY_CAST( CAST(N'<x>' + REPLACE(Name,N' ',N'</x><x>') + N'</x>' AS XML).value('/x[2]','nvarchar(max)') AS INT) IS NULL
	                                    THEN 0
	                                    ELSE TRY_CAST( CAST(N'<x>' + REPLACE(Name,N' ',N'</x><x>') + N'</x>' AS XML).value('/x[2]','nvarchar(max)') AS INT)
	                                    END");
        }

        public override void Down()
        {
            DropColumn("finance.PlanDefinitions", "TermInYears");
        }
    }
}
