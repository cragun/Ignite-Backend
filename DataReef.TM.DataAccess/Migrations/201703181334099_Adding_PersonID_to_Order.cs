namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Adding_PersonID_to_Order : DbMigration
    {
        public override void Up()
        {
            AddColumn("commerce.Orders", "PersonID", c => c.Guid(nullable: false));
            AlterColumn("commerce.Orders", "CSVFilePath", c => c.String(maxLength: 250));
        }
        
        public override void Down()
        {
            AlterColumn("commerce.Orders", "CSVFilePath", c => c.String());
            DropColumn("commerce.Orders", "PersonID");
        }
    }
}
