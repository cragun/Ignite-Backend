namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Adding_APNDeviceToken_to_Devices : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Devices", "APNDeviceToken", c => c.String(maxLength: 250));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Devices", "APNDeviceToken");
        }
    }
}
