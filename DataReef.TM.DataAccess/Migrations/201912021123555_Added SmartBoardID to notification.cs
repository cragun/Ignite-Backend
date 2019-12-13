namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedSmartBoardIDtonotification : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Notifications", "SmartBoardID", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Notifications", "SmartBoardID");
        }
    }
}
