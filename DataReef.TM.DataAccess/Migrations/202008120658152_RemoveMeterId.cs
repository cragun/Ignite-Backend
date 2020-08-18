namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveMeterId : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Appointments", "MeterId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Appointments", "MeterId", c => c.String());
        }
    }
}
