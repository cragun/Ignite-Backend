namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_Territory_PropertyCount : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Territories", "PropertyCount", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Territories", "PropertyCount");
        }
    }
}
