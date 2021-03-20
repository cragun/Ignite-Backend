namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddThirdPartyType : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Appointments", "PropertyType", c => c.Int(nullable: false));
            AddColumn("dbo.Properties", "PropertyType", c => c.Int(nullable: false));
            AddColumn("dbo.PropertyNotes", "PropertyType", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.PropertyNotes", "PropertyType");
            DropColumn("dbo.Properties", "PropertyType");
            DropColumn("dbo.Appointments", "PropertyType");
        }
    }
}
