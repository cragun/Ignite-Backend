namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InvertersSystemSizeRange : DbMigration
    {
        public override void Up()
        {
            AddColumn("solar.Inverters", "MinSystemSizeInWatts", c => c.Int(nullable: false));
            AddColumn("solar.Inverters", "MaxSystemSizeInWatts", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("solar.Inverters", "MaxSystemSizeInWatts");
            DropColumn("solar.Inverters", "MinSystemSizeInWatts");
        }
    }
}
