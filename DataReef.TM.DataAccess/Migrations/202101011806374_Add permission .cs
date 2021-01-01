namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Addpermission : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OUs", "Permissions", c => c.Long(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.OUs", "Permissions");
        }
    }
}
