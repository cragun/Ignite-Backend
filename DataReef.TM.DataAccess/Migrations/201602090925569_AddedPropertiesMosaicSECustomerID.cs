namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedPropertiesMosaicSECustomerID : DbMigration
    {
        public override void Up()
        {
            RenameColumn("dbo.Properties", "SunEdisonCustomerID", "NetSuiteSECustomerID");
            AddColumn("dbo.Properties", "MosaicSECustomerID", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Properties", "MosaicSECustomerID");
            RenameColumn("dbo.Properties", "NetSuiteSECustomerID", "SunEdisonCustomerID");
        }
    }
}
