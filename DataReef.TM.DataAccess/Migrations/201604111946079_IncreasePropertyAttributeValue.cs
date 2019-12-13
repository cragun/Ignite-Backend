namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IncreasePropertyAttributeValue : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.PropertyAttributes", "Value", c => c.String(maxLength: 150));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.PropertyAttributes", "Value", c => c.String(maxLength: 50));
        }
    }
}
