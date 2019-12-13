namespace DataReef.TM.DataAccess.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class PersistPropertyLatestStatus : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Properties", "LatestStatus", c => c.Int(nullable: false));
            Sql("update Properties set LatestStatus = ISNULL((select top 1 Status from Inquiries where PropertyId = Properties.Guid order by DateCreated desc), 0)");
        }
        
        public override void Down()
        {
            DropColumn("dbo.Properties", "LatestStatus");
        }
    }
}
