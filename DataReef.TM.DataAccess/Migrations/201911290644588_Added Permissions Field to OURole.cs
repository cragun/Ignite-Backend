namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedPermissionsFieldtoOURole : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OURoles", "Permissions", c => c.Long(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.OURoles", "Permissions");
        }
    }
}
