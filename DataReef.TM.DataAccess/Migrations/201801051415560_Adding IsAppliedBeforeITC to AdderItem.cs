namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class AddingIsAppliedBeforeITCtoAdderItem : DbMigration
    {
        public override void Up()
        {
            AddColumn("solar.AdderItems", "IsAppliedBeforeITC", c => c.Boolean(nullable: false));
        }

        public override void Down()
        {
            DropColumn("solar.AdderItems", "IsAppliedBeforeITC");
        }
    }
}
