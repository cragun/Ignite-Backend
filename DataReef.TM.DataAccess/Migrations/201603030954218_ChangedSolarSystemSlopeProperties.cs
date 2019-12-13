namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangedSolarSystemSlopeProperties : DbMigration
    {
        public override void Up()
        {
            AddColumn("solar.Systems", "ApplyConsumptionSlope", c => c.Boolean(nullable: false));

            Sql("UPDATE solar.Systems SET ApplyConsumptionSlope = 1 where  Guid in (select Guid from solar.Systems where AverageSlopeIndex != -1 or MonthlySlopeIndex != -1)");

            DropColumn("solar.Systems", "MonthlySlopeIndex");
            DropColumn("solar.Systems", "AverageSlopeIndex");
        }
        
        public override void Down()
        {
            AddColumn("solar.Systems", "AverageSlopeIndex", c => c.Int(nullable: false));
            AddColumn("solar.Systems", "MonthlySlopeIndex", c => c.Int(nullable: false));
            DropColumn("solar.Systems", "ApplyConsumptionSlope");
        }
    }
}
