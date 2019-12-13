namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPhotosAdminrole : DbMigration
    {
        public override void Up()
        {
            var sqlQuery = @"INSERT INTO [OURoles] ([Guid]
                              ,[IsActive]
                              ,[IsAdmin]
                              ,[Name]
                              ,[Flags]
                              ,[TenantID]
                              ,[DateCreated]
                              ,[DateLastModified]
                              ,[CreatedByName]
                              ,[CreatedByID]
                              ,[LastModifiedBy]
                              ,[LastModifiedByName]
                              ,[Version]
                              ,[IsDeleted]
                              ,[ExternalID]
                              ,[TagString]
                              ,[IsOwner]
                              ,[RoleType])
                        VALUES (
                            '7d28440d-34b4-4eac-924c-0f4e235ff486'
                              ,1
                              ,0
                              ,'Photos Admin'
                              ,NULL
                              ,0
                              ,GETDATE()
                              ,NULL
                              ,NULL
                              ,NULL
                              ,NULL
                              ,NULL
                              ,0
                              ,0
                              ,NULL
                              ,NULL
                              ,0
                              ,128
                        )";

            Sql(sqlQuery);
        }

        public override void Down()
        {
            var sqlQuery = @"   DELETE FROM OUAssociations WHERE OURoleID = '7d28440d-34b4-4eac-924c-0f4e235ff486';
                                DELETE FROM OURoles WHERE[Guid] = '7d28440d-34b4-4eac-924c-0f4e235ff486'";
            Sql(sqlQuery);
        }
    }
}
