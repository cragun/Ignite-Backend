namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AdderItems_UpdateModel_Incentives : DbMigration
    {
        public override void Up()
        {
            AddColumn("solar.AdderItems", "Type", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("solar.AdderItems", "Type");
        }
    }
}
