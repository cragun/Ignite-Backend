namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddFcmToken : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.People", "fcm_token", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.People", "fcm_token");
        }
    }
}
