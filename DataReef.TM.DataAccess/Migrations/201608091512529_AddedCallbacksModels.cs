namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedCallbacksModels : DbMigration
    {
        public override void Up()
        {
            AddColumn("Spruce.QuoteResponses", "CallbackJSON", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("Spruce.QuoteResponses", "CallbackJSON");
        }
    }
}
