namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class OrderOrderStatus : DbMigration
    {
        public override void Up()
        {
            AddColumn("commerce.Orders", "Status", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("commerce.Orders", "Status");
        }
    }
}
