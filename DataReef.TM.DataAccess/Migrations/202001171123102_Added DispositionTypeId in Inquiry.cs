namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedDispositionTypeIdinInquiry : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Inquiries", "DispositionTypeId", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Inquiries", "DispositionTypeId");
        }
    }
}
