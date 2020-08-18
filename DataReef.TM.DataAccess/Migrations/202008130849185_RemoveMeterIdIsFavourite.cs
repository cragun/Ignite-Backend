namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveMeterIdIsFavourite : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Appointments", "IsFavourite");
            DropColumn("dbo.Appointments", "MeterID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Appointments", "MeterID", c => c.String());
            AddColumn("dbo.Appointments", "IsFavourite", c => c.Boolean(nullable: false));
        }
    }
}
