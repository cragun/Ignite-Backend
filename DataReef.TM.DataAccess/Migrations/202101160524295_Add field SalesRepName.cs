namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddfieldSalesRepName : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Inquiries", "SalesRepName", c => c.String(maxLength: 100));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Inquiries", "SalesRepName");
        }
    }
}
