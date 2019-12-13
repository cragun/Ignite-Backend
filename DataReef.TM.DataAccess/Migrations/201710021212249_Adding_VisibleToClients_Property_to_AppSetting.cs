namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class Adding_VisibleToClients_Property_to_AppSetting : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AppSettings", "VisibleToClients", c => c.Boolean(nullable: false, defaultValue: false));
            Sql(@"INSERT INTO AppSettings
                (Guid, [Key], Value, Name, TenantID, DateCreated, Version, IsDeleted, VisibleToClients)
                VALUES
                (NEWID(), 'Solar.Financing.Mortgage.Online.Url', 'https://myloan.primeres.com/#/signup?referrerId=dworthington@primeres.com', 'Solar.Financing.Mortgage.Online.Url', 0, GETUTCDATE(), 0, 0, 1)");
        }

        public override void Down()
        {
            DropColumn("dbo.AppSettings", "VisibleToClients");
        }
    }
}
