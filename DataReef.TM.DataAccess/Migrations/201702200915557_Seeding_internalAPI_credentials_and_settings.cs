namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class Seeding_internalAPI_credentials_and_settings : DbMigration
    {
        private static string userId = "58C12CDB-29D4-4909-BC69-3594FF589ADC";
        private static string key = "Auth Token Validity";

        public override void Up()
        {
            Sql($@"INSERT INTO [PersonSettings]
                ([Guid], PersonID, [Value], [Name], [TenantID], [DateCreated], [Version], [IsDeleted], [ValueType], [Group])
                VALUES
                (NEWID(), '{userId}', '365', '{key}', 0, GETUTCDATE(), 1, 0, 0, 1)");

            Sql($@"INSERT INTO [Credentials]
                ([Guid], [UserID], [PersonID], UserName, PasswordHashed, Salt, RequiresPasswordChange, TenantID, DateCreated, [Version], IsDeleted)
                VALUES
                (NEWID(), '{userId}', '{userId}', 'internalAPI@datareef.com', '2MW6OUob2hJxV3iZJ/vTIiA+TgBulES8jCO+T8mcANQ=', 'G3zSxDMTTD5QjGg3/Gj3ZRueCKES9qb+', 0, 0, GETUTCDATE(), 1, 0)");
        }

        public override void Down()
        {
            Sql($@"DELETE FROM [PersonSettings]
                   WHERE PersonID = '{userId}' AND [Name] = '{key}'");

            Sql($@"DELETE FROM [Credentials]
                   WHERE PersonID = '{userId}'");
        }
    }
}
