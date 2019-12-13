namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddingPropertyNotes : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Properties", "Notes", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Properties", "Notes");
        }
    }
}
