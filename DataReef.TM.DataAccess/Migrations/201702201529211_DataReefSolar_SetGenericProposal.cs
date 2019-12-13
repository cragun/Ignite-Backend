namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DataReefSolar_SetGenericProposal : DbMigration
    {
        public override void Up()
        {
            Sql(@"declare @dataReefSolarOUID uniqueidentifier = (select top 1 Guid from dbo.OUs where Name = 'DataReef Solar')
                  declare @ouSettingID uniqueidentifier = (select top 1 Guid from dbo.OUSettings where OUID = @dataReefSolarOUID and Name = 'ProposalTemplates')

                update dbo.OUSettings
                set Value = REPLACE(value, '95f874d5-92d0-4fc5-9704-47956c7549cb', 'b41eda2d-416b-4ba2-8c24-c83eeee65d35')
                where Guid = @ouSettingID");
        }

        public override void Down()
        {
            Sql(@"declare @dataReefSolarOUID uniqueidentifier = (select top 1 Guid from dbo.OUs where Name = 'DataReef Solar')
                  declare @ouSettingID uniqueidentifier = (select top 1 Guid from dbo.OUSettings where OUID = @dataReefSolarOUID and Name = 'ProposalTemplates')

                update dbo.OUSettings
                set Value = REPLACE(value, 'b41eda2d-416b-4ba2-8c24-c83eeee65d35', '95f874d5-92d0-4fc5-9704-47956c7549cb')
                where Guid = @ouSettingID");
        }
    }
}
