namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Removing_OUSettingValueIsDefault : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.OUSettings", "ValueIsDefault");
        }
        
        public override void Down()
        {
            AddColumn("dbo.OUSettings", "ValueIsDefault", c => c.Boolean(nullable: false));
        }
    }
}
