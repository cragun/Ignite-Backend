namespace DataReef.TM.DataAccess.Migrations
{
    using DataReef.TM.Models.Enums;
    using System;
    using System.Data.Entity.Migrations;

    public partial class Adding_VelocifyRequest_Source : DbMigration
    {
        public override void Up()
        {
            AddColumn("PRMI.VelocifyRequests", "Source", c => c.Int(nullable: false, defaultValue: (int)VelocifyRequestSourceType.LegionApp));
        }

        public override void Down()
        {
            DropColumn("PRMI.VelocifyRequests", "Source");
        }
    }
}
