namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddEscalationRateToSolarSystem : DbMigration
    {
        public override void Up()
        {
            AddColumn("solar.Systems", "EscalationRate", c => c.Double());
        }
        
        public override void Down()
        {
            DropColumn("solar.Systems", "EscalationRate");
        }
    }
}
