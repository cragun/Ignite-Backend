namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddingSmartBoardIdtoPropertyBase : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Properties", "SmartBoardId", c => c.Long());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Properties", "SmartBoardId");
        }
    }
}
