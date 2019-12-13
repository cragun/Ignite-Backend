namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class Adding_Adder_IsRebate : DbMigration
    {
        public override void Up()
        {
            AddColumn("solar.AdderItems", "IsRebate", c => c.Boolean());
        }

        public override void Down()
        {
            DropColumn("solar.AdderItems", "IsRebate");
        }
    }
}
