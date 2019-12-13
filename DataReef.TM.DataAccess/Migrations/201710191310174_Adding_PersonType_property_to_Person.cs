namespace DataReef.TM.DataAccess.Migrations
{
    using DataReef.TM.Models.Enums;
    using System;
    using System.Data.Entity.Migrations;

    public partial class Adding_PersonType_property_to_Person : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.People", "PersonType", c => c.Int(nullable: false, defaultValue: (int)PersonType.Regular));
        }

        public override void Down()
        {
            DropColumn("dbo.People", "PersonType");
        }
    }
}
