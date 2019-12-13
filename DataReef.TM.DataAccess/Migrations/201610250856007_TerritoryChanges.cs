namespace DataReef.TM.DataAccess.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class TerritoryChanges : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Territories", "PropertyCount");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Territories", "PropertyCount", c => c.Int(nullable: false));
        }
    }
}
