namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAgreementBaseUrlSetting : DbMigration
    {
        public override void Up()
        {
            var sql = @"INSERT INTO [OUSettings] ([Guid]
      ,[OUID]
      ,[Value]
      ,[Group]
      ,[Inheritable]
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
      ,[ValueType])
    VALUES (
        NEWID(),
        '3f78b1b0-c0c5-4987-a5d5-32ee1c893460',
        'http://ignite-proposals-stage.s3-website-us-west-2.amazonaws.com/trismart-solar/install/',
        2,
        1,
        'Proposal.Agreements.BaseUrl',
        NULL,
        0,
        GETDATE(),
        NULL,
        NULL,
        NULL,
        NULL,
        NULL,
        0,
        0,
        NULL,
        NULL,
        1
    )";
            Sql(sql);
        }
        
        public override void Down()
        {
            var sql = @"DELETE FROM [OUSettings] WHERE [Name]='Proposal.Agreements.BaseUrl'";

            Sql(sql);
        }
    }
}
