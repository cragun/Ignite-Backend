namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_PropertyID_Migration : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Notifications", "PropertyID", c => c.Guid(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Notifications", "PropertyID");
        }
    }
}
