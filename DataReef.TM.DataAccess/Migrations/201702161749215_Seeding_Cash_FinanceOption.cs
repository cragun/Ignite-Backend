namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class Seeding_Cash_FinanceOption : DbMigration
    {
        private static string genericProviderId = "ebb25b84-394b-4101-b42d-783f83d9ef8a";
        public override void Up()
        {
            Sql($@"INSERT [finance].[Providers] ([Guid], [ImageUrl], [Name], [TenantID], [DateCreated], [Version], [IsDeleted])
               VALUES (N'{genericProviderId}', N'',  N'Generic provider', 0, CAST(N'2017-02-16 23:05:57.713' AS DateTime), 1, 0)");

            Sql($@"insert into finance.planDefinitions ( Guid, ProviderID, Type, IsDisabled, Name, TenantID, DateCreated, Version, IsDeleted)
                  select NewID() as Guid, '{genericProviderId}' as ProviderID, 4 as Type, 0 as IsDisabled, 'Cash' as Name, 0 as TenantID, GetUTCDate() as DateCreated, 1 as Version, 0 as IsDeleted");
        }

        public override void Down()
        {
            Sql($@"DELETE FROM [finance].[planDefinitions] WHERE ProviderID = '{genericProviderId}'
                   DELETE FROM [finance].[Providers] WHERE Guid = '{genericProviderId}'");
        }
    }
}
