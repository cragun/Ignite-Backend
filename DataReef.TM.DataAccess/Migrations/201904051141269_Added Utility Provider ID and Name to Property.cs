namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedUtilityProviderIDandNametoProperty : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Properties", "UtilityProviderID", c => c.String(maxLength: 200));
            AddColumn("dbo.Properties", "UtilityProviderName", c => c.String(maxLength: 200));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Properties", "UtilityProviderName");
            DropColumn("dbo.Properties", "UtilityProviderID");
        }
    }
}
