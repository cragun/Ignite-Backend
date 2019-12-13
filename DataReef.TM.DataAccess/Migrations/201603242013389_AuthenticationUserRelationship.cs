namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AuthenticationUserRelationship : DbMigration
    {
        public override void Up()
        {
            AddForeignKey("dbo.Authentications", "UserID", "dbo.Users", "Guid");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Authentications", "UserID", "dbo.Users");
        }
    }
}
