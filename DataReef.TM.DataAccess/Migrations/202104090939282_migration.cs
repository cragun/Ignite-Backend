namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class migration : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Properties", "SunnovaContactsResponse", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Properties", "SunnovaContactsResponse");
        }
    }
}
