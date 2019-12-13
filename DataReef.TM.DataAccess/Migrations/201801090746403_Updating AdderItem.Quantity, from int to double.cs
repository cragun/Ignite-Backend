namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class UpdatingAdderItemQuantityfrominttodouble : DbMigration
    {
        public override void Up()
        {
            AlterColumn("solar.AdderItems", "Quantity", c => c.Double(nullable: false));
        }

        public override void Down()
        {
            AlterColumn("solar.AdderItems", "Quantity", c => c.Int(nullable: false));
        }
    }
}
