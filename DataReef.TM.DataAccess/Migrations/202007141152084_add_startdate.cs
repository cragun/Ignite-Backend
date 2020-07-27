namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_startdate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "StartDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Users", "StartDate");
        }
    }
}
