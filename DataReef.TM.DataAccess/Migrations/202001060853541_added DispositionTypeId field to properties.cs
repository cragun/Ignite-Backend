namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedDispositionTypeIdfieldtoproperties : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Properties", "DispositionTypeId", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Properties", "DispositionTypeId");
        }
    }
}
