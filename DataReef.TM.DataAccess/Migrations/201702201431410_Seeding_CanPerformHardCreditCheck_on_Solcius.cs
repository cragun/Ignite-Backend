namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class Seeding_CanPerformHardCreditCheck_on_Solcius : DbMigration
    {
        public override void Up()
        {
            Sql(@"INSERT INTO OUSettings
                 ([Guid], [OUID], [Value], [Group], [Inheritable], [Name], [TenantID], [DateCreated], [Version], [IsDeleted], [ValueType])
                 VALUES
                 (NEWID(), 'c5f80842-f4eb-425e-a93d-0571e357d18a', '1', 1, 1, 'CanPerformHardCreditCheck', 0, GETUTCDATE(), 1, 0, 1)");
        }

        public override void Down()
        {
            Sql(@"DELETE FROM OUSettings
                  WHERE [OUID] = 'c5f80842-f4eb-425e-a93d-0571e357d18a' AND [Name] = 'CanPerformHardCreditCheck'");
        }
    }
}
