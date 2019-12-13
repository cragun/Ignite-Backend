namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedRootOrganizationNameToOU : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OUs", "RootOrganizationName", c => c.String());
            Sql("UPDATE Ous SET Ous.RootOrganizationName = rootOus.Name FROM dbo.OUs AS Ous JOIN dbo.OUs AS rootOus ON Ous.RootOrganizationID = rootOus.Guid ");
        }
        
        public override void Down()
        {
            DropColumn("dbo.OUs", "RootOrganizationName");
        }
    }
}
