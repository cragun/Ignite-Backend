namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TerritoryPropertCount_Nullable : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Territories", "PropertyCount", c => c.Int());
            Sql(@"update dbo.Territories set PropertyCount = null where PropertyCount = 0");
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Territories", "PropertyCount", c => c.Int(nullable: false));
        }
    }
}
