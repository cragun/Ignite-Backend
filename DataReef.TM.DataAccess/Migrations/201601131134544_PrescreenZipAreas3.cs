namespace DataReef.TM.DataAccess.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class PrescreenZipAreas3 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AreaPurchases", "Status", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AreaPurchases", "Status");
        }
    }
}
