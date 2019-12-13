namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class Adding_FinancePlan_FinancePlanDefinitionId : DbMigration
    {
        public override void Up()
        {
            AddColumn("solar.FinancePlans", "FinancePlanDefinitionID", c => c.Guid());
            CreateIndex("solar.FinancePlans", "FinancePlanDefinitionID");
            AddForeignKey("solar.FinancePlans", "FinancePlanDefinitionID", "finance.PlanDefinitions", "Guid");

            Sql(@"insert into finance.planDefinitions ( Guid,ProviderID,Type,IsDisabled,Name,TenantID,DateCreated,Version,IsDeleted)    
                  select NewID() as Guid,(select top 1 guid from finance.providers where name='Spruce Financial')  as ProviderID,2 as Type, 1 as IsDisabled, 'Spruce 12 / 3.99%' as Name,0 as TenantID,GetUTCDate() as DateCreated,1 as Version,0 as IsDeleted");

            Sql(@"insert into finance.planDefinitions ( Guid,ProviderID,Type,IsDisabled,Name,TenantID,DateCreated,Version,IsDeleted)    
                  select NewID() as Guid,(select top 1 guid from finance.providers where name='Mosaic')  as ProviderID,2 as Type, 1 as IsDisabled, 'Mosaic 12 / 3.99 %' as Name,0 as TenantID,GetUTCDate() as DateCreated,1 as Version,0 as IsDeleted");

            Sql(@"INSERT [finance].[Providers] ([Guid], [ImageUrl], [Name], [Flags], [TenantID], [DateCreated], [DateLastModified], [CreatedByName], [CreatedByID], [LastModifiedBy], [LastModifiedByName], [Version], [IsDeleted], [ExternalID], [TagString])
               VALUES (N'bf5540b1-6ccf-4057-b2db-ca01048ba377', N'',  N'OBSOLETE PROVIDER (DO NOT USE)', NULL, 0, CAST(N'2017-02-06 23:05:57.713' AS DateTime), NULL, NULL, NULL, NULL, NULL, 1, 0, NULL, NULL)");

            Sql(@"insert into finance.planDefinitions ( Guid,ProviderID,Type,IsDisabled,Name,TenantID,DateCreated,Version,IsDeleted)
                  select NewID() as Guid, 'bf5540b1-6ccf-4057-b2db-ca01048ba377' as ProviderID, 2 as Type, 1 as IsDisabled, 'OBSOLETE PLAN (DO NOT USE)' as Name,0 as TenantID,GetUTCDate() as DateCreated,1 as Version,0 as IsDeleted");

            // Migrate Finance Plan Definitions
            var sqlMigrationPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../..", "Database/SQL/Migrations/Migrate_FinancePlanDefinitions.sql");
            SqlFile(sqlMigrationPath);
        }

        public override void Down()
        {
            DropForeignKey("solar.FinancePlans", "FinancePlanDefinitionID", "finance.PlanDefinitions");
            DropIndex("solar.FinancePlans", new[] { "FinancePlanDefinitionID" });
            DropColumn("solar.FinancePlans", "FinancePlanDefinitionID");

            Sql("DELETE FROM finance.planDefinitions WHERE NAME IN ('Spruce 12 / 3.99%', 'Mosaic 12 / 3.99 %', 'OBSOLETE PLAN (DO NOT USE)')");
            Sql("DELETE FROM [finance].[Providers] WHERE Guid = 'bf5540b1-6ccf-4057-b2db-ca01048ba377'");
        }
    }
}
