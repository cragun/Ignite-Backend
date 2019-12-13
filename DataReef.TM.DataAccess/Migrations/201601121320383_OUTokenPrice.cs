namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class OUTokenPrice : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OUs", "TokenPriceInDollars", c => c.Single());
        }
        
        public override void Down()
        {
            DropColumn("dbo.OUs", "TokenPriceInDollars");
        }
    }
}
