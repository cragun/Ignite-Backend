namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddingAddressIDtoProperty : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Properties", "AddressID", c => c.String(maxLength: 50));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Properties", "AddressID");
        }
    }
}
