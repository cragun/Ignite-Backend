namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PropertyPrescreens : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PropertyAttributes", "ExpiryMinutes", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.PropertyAttributes", "ExpiryMinutes");
        }
    }
}
