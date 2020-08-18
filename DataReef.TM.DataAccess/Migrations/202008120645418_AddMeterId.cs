namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddMeterId : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Appointments", "MeterId", c => c.Int(nullable: false));
        }

        public override void Down()
        {
            DropColumn("dbo.Appointments", "MeterId");
        }
    }
}
