namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Addmigrationtables : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.People", "StartDate", c => c.DateTime());
            AddColumn("dbo.OUs", "Permissions", c => c.Long(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.People", "StartDate", c => c.DateTime(nullable: false));
            DropColumn("dbo.OUs", "Permissions");
        }
    }
}
