namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class AddingDynamicSettingsJSONtoAdderItem : DbMigration
    {
        public override void Up()
        {
            AddColumn("solar.AdderItems", "DynamicSettingsJSON", c => c.String());
        }

        public override void Down()
        {
            DropColumn("solar.AdderItems", "DynamicSettingsJSON");
        }
    }
}
