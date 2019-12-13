namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Adding_AdderItemTemplateID : DbMigration
    {
        public override void Up()
        {
            // Will delete all AdderItems before making the change
            Sql("DELETE FROM solar.AdderItems");

            AddColumn("solar.AdderItems", "TemplateID", c => c.Guid(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("solar.AdderItems", "TemplateID");
        }
    }
}
