namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AdderItem_MakeHistoryFieldsNullable : DbMigration
    {
        public override void Up()
        {
            AlterColumn("solar.AdderItems", "AllowsQuantitySelection", c => c.Boolean());
            AlterColumn("solar.AdderItems", "CanBePaidForByRep", c => c.Boolean());
        }
        
        public override void Down()
        {
            AlterColumn("solar.AdderItems", "CanBePaidForByRep", c => c.Boolean(nullable: false));
            AlterColumn("solar.AdderItems", "AllowsQuantitySelection", c => c.Boolean(nullable: false));
        }
    }
}
