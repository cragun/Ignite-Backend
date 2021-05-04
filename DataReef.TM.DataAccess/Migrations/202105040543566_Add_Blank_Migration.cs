namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_Blank_Migration : DbMigration
    {
        public override void Up()
        {
            AddForeignKey("dbo.Notifications", "PropertyID", "dbo.Properties", "Guid");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Notifications", "PropertyID", "dbo.Properties");
        }
    }
}
