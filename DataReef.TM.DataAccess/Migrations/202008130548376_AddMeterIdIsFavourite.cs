namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddMeterIdIsFavourite : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Appointments", "IsFavourite", c => c.Boolean(nullable: false));
            AddColumn("dbo.Appointments", "MeterID", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Appointments", "MeterID");
            DropColumn("dbo.Appointments", "IsFavourite");
        }
    }
}
