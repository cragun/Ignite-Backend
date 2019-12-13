namespace DataReef.TM.DataAccess.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class PropertiesRemoveSortOrder : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Properties", "SortOrder");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Properties", "SortOrder", c => c.Int(nullable: false));
        }
    }
}
