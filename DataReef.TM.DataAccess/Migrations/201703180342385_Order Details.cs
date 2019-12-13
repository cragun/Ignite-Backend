namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class OrderDetails : DbMigration
    {
        public override void Up()
        {
            AddColumn("commerce.Orders", "WorkflowId", c => c.Guid());
        }
        
        public override void Down()
        {
            DropColumn("commerce.Orders", "WorkflowId");
        }
    }
}
