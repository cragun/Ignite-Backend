namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Adding_LoanExtraCosts_and_LoanTaxRate_to_SolarSystem : DbMigration
    {
        public override void Up()
        {
            AddColumn("solar.Systems", "LoanExtraCosts", c => c.Decimal(precision: 18, scale: 2, defaultValue: 0));
            AddColumn("solar.Systems", "LoanTaxRate", c => c.Decimal(precision: 18, scale: 2, defaultValue: 0));
        }
        
        public override void Down()
        {
            DropColumn("solar.Systems", "LoanTaxRate");
            DropColumn("solar.Systems", "LoanExtraCosts");
        }
    }
}
