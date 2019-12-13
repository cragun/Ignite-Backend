namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Increasing_precision_for_Obstruction_Points : DbMigration
    {
        public override void Up()
        {
            AlterColumn("solar.ObstructionPoints", "PointX", c => c.Double(nullable: false));
            AlterColumn("solar.ObstructionPoints", "PointY", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("solar.ObstructionPoints", "PointY", c => c.Single(nullable: false));
            AlterColumn("solar.ObstructionPoints", "PointX", c => c.Single(nullable: false));
        }
    }
}
