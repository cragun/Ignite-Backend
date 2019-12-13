namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class Adding_internaAPI_PersonAndUser : DbMigration
    {
        private static string userId = "58C12CDB-29D4-4909-BC69-3594FF589ADC";

        public override void Up()
        {
            Sql(@"INSERT INTO People 
                ([Guid], [EmailAddressString], FirstName, LastName, DateCreated, TenantID, Version, IsDeleted)
                VALUES
                ('" + userId + @"', 'internalAPI@datareef.com', 'Internal', 'API', '2016-08-15', 0, 0, 0)

                INSERT INTO Users
                ([Guid], PersonID, IsActive, IsDisabled, TenantID, DateCreated, Version, IsDeleted, NumberOfDevicesAllowed)
                VALUES
                ('" + userId + @"', '" + userId + @"', 1, 0, 0, '2016-08-15', 0, 0, 1)");
        }

        public override void Down()
        {
            Sql(@"DELETE FROM Users WHERE [Guid] = '" + userId + @"'
                  DELETE FROM People WHERE[Guid] = '" + userId + @"'");
        }
    }
}
