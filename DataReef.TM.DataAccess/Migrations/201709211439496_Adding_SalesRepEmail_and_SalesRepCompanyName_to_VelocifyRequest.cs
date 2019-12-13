namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Adding_SalesRepEmail_and_SalesRepCompanyName_to_VelocifyRequest : DbMigration
    {
        public override void Up()
        {
            AddColumn("PRMI.VelocifyRequests", "SalesRepCompanyName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("PRMI.VelocifyRequests", "SalesRepCompanyName");
        }
    }
}
