namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedExpirationDateToToken : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ApiTokens", "ExpirationDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ApiTokens", "ExpirationDate");
        }
    }
}
