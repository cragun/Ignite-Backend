namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Removestartdatefromuser : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Users", "StartDate");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Users", "StartDate", c => c.DateTime());
        }
    }
}
