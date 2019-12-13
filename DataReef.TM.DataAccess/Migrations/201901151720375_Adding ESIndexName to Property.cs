namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddingESIndexNametoProperty : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Properties", "ESIndexName", c => c.String(maxLength: 15));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Properties", "ESIndexName");
        }
    }
}
