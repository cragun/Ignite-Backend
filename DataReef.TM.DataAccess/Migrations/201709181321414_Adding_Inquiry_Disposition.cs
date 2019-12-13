namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Adding_Inquiry_Disposition : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Properties", "LatestDisposition", c => c.String(maxLength: 50));
            AddColumn("dbo.Inquiries", "Disposition", c => c.String(maxLength: 50));
            CreateIndex("dbo.Inquiries", "Disposition", name: "IDX_INQUIRY_DISPOSITION");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Inquiries", "IDX_INQUIRY_DISPOSITION");
            DropColumn("dbo.Inquiries", "Disposition");
            DropColumn("dbo.Properties", "LatestDisposition");
        }
    }
}
