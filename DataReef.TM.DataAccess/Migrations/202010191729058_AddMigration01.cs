namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddMigration01 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.QuotasCommitments", "EndDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.QuotasCommitments", "EndDate", c => c.DateTime());
        }
    }
}
