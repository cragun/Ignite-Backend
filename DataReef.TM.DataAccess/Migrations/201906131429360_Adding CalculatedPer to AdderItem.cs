namespace DataReef.TM.DataAccess.Migrations
{
    using DataReef.TM.Models.Enums;
    using System.Data.Entity.Migrations;

    public partial class AddingCalculatedPertoAdderItem : DbMigration
    {
        public override void Up()
        {
            AddColumn("solar.AdderItems", "CalculatedPer", c => c.Int(defaultValue: (int)AdderItemCalculatedPerType.None));
        }

        public override void Down()
        {
            DropColumn("solar.AdderItems", "CalculatedPer");
        }
    }
}
