namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AdderItem_AddDescription : DbMigration
    {
        public override void Up()
        {
            AddColumn("solar.AdderItems", "Description", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("solar.AdderItems", "Description");
        }
    }
}
