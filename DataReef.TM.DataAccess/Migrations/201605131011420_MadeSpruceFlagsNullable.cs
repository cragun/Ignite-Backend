namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MadeSpruceFlagsNullable : DbMigration
    {
        public override void Up()
        {
            AlterColumn("solar.Systems", "Spruce12YearLoanIsSelected", c => c.Boolean());
            AlterColumn("solar.Systems", "Spruce20YearLoanIsSelected", c => c.Boolean());
        }
        
        public override void Down()
        {
            AlterColumn("solar.Systems", "Spruce20YearLoanIsSelected", c => c.Boolean(nullable: false));
            AlterColumn("solar.Systems", "Spruce12YearLoanIsSelected", c => c.Boolean(nullable: false));
        }
    }
}
