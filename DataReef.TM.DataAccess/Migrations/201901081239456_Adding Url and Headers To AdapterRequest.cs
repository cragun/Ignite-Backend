namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddingUrlandHeadersToAdapterRequest : DbMigration
    {
        public override void Up()
        {
            AddColumn("FI.AdapterRequests", "Url", c => c.String());
            AddColumn("FI.AdapterRequests", "Headers", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("FI.AdapterRequests", "Headers");
            DropColumn("FI.AdapterRequests", "Url");
        }
    }
}
