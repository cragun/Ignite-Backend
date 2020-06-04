namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddBuildVersioninpeopletable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.People", "BuildVersion", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.People", "BuildVersion");
        }
    }
}
