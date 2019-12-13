namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveMosaicSECustomerID : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Properties", "MosaicSECustomerID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Properties", "MosaicSECustomerID", c => c.String());
        }
    }
}
