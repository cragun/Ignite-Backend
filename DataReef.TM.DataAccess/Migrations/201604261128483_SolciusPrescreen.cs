namespace DataReef.TM.DataAccess.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class SolciusPrescreen : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PrescreenInstants", "Source", c => c.Int(nullable: false));
            Sql("UPDATE dbo.PrescreenInstants SET Source = 1");
        }
        
        public override void Down()
        {
            DropColumn("dbo.PrescreenInstants", "Source");
        }
    }
}
