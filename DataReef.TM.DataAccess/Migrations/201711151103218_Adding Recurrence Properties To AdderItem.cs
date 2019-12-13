namespace DataReef.TM.DataAccess.Migrations
{
    using DataReef.TM.Models.Enums;
    using System;
    using System.Data.Entity.Migrations;

    public partial class AddingRecurrencePropertiesToAdderItem : DbMigration
    {
        public override void Up()
        {
            AddColumn("solar.AdderItems", "RecurrenceType", c => c.Int(nullable: false, defaultValue: (int)AdderItemRecurrenceType.OneTime));
            AddColumn("solar.AdderItems", "RecurrenceStart", c => c.Int(nullable: false));
            AddColumn("solar.AdderItems", "RecurrencePeriod", c => c.Int(nullable: false));
        }

        public override void Down()
        {
            DropColumn("solar.AdderItems", "RecurrencePeriod");
            DropColumn("solar.AdderItems", "RecurrenceStart");
            DropColumn("solar.AdderItems", "RecurrenceType");
        }
    }
}
