namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SpruceModelUpdates : DbMigration
    {
        public override void Up()
        {
            AddColumn("Spruce.QuoteRequests", "CallbackJSON", c => c.String());
            DropColumn("Spruce.GenDocsRequests", "CallbackJSON");
            DropColumn("Spruce.QuoteResponses", "CallbackJSON");
        }
        
        public override void Down()
        {
            AddColumn("Spruce.QuoteResponses", "CallbackJSON", c => c.String());
            AddColumn("Spruce.GenDocsRequests", "CallbackJSON", c => c.String());
            DropColumn("Spruce.QuoteRequests", "CallbackJSON");
        }
    }
}
