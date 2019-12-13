namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FieldDisplayNameAndValueAreRequired : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Fields", "DisplayName", c => c.String(nullable: false, maxLength: 100));
            AlterColumn("dbo.Fields", "Value", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Fields", "Value", c => c.String());
            AlterColumn("dbo.Fields", "DisplayName", c => c.String(maxLength: 100));
        }
    }
}
