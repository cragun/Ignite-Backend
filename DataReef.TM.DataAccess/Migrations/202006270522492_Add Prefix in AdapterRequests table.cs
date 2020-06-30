namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPrefixinAdapterRequeststable : DbMigration
    {
        public override void Up()
        {
            AddColumn("FI.AdapterRequests", "Prefix", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("FI.AdapterRequests", "Prefix");
        }
    }
}
