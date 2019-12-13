namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FirstMigration : DbMigration
    {
        public override void Up()
        {
            Sql("Update solar.Arrays set RidgeLineAzimuth = 0 where RidgeLineAzimuth Is Null");
            AlterColumn("solar.Arrays", "RidgeLineAzimuth", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("solar.Arrays", "RidgeLineAzimuth", c => c.Int(nullable: false));
        }
    }
}
