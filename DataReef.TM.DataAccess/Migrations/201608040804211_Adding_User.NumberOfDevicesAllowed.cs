namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Adding_UserNumberOfDevicesAllowed : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "NumberOfDevicesAllowed", c => c.Int(nullable: false, defaultValue: 1));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Users", "NumberOfDevicesAllowed");
        }
    }
}
