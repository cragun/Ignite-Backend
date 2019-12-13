namespace DataReef.TM.DataAccess.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class BatchPrescreenChanges1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PrescreenBatches", "Source", c => c.Int(nullable: false));
            Sql("UPDATE dbo.PrescreenBatches SET status = 5 where status in (1,2,3)");
        }
        
        public override void Down()
        {
            DropColumn("dbo.PrescreenBatches", "Source");
        }
    }
}
