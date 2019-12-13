namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeCurrentLocationAccuracyfrominttofloat : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.CurrentLocations", "Accuracy", c => c.Single(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.CurrentLocations", "Accuracy", c => c.Int(nullable: false));
        }
    }
}
