namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedESID : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Appointments", "ESID", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Appointments", "ESID");
        }
    }
}
