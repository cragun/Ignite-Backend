namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Remove_ExpireMinutes_from_PropertyAttribute : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.PropertyAttributes", "ExpiryMinutes");
        }
        
        public override void Down()
        {
            AddColumn("dbo.PropertyAttributes", "ExpiryMinutes", c => c.Int(nullable: false));
        }
    }
}
