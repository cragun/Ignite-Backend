namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Adding_FinancePlansSettingsJSON_To_SolarSystem : DbMigration
    {
        public override void Up()
        {
            AddColumn("solar.Systems", "FinancePlansSettingsJSON", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("solar.Systems", "FinancePlansSettingsJSON");
        }
    }
}
