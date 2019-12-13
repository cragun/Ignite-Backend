namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Removing_Inquiry_Notes_lenght_restriction : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Inquiries", "Notes", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Inquiries", "Notes", c => c.String(maxLength: 500));
        }
    }
}
