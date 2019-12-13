namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateFunctionandSPGetOUSettingsMany : DbMigration
    {
        public override void Up()
        {
            var sql = @"CREATE FUNCTION [dbo].[GetOUSettings]
                    (
                        @OUID VARCHAR(50),
                        @IncludeDeleted BIT
                    )
                    returns TABLE AS
                    RETURN
                    SELECT S.*, T.[Level]
                        FROM OUSettings S
                        INNER JOIN 
                        (
                            SELECT T.Guid, ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) as [Level]
                            from OUTreeUP(@OUID) T
                        ) T
                        ON S.OUID = T.Guid
                        WHERE (@IncludeDeleted = 1 OR (@IncludeDeleted = 0 AND S.IsDeleted = 0) )
                    GO

                    CREATE PROCEDURE [dbo].[proc_OUSettingsMany]
	                    @OUIDs GuidList READONLY,
	                    @IncludeDeleted BIT = 0
                    AS
                    BEGIN

                    SELECT O.Id AS OrgID, SETT.* FROM @OUIDs O
                    OUTER APPLY GetOUSettings(O.Id, 0) SETT

                    END
                    GO";

            Sql(sql);
        }
        
        public override void Down()
        {
            var sql = @"
            DROP FUNCTION IF EXISTS [dbo].[GetOUSettings]
            GO
            DROP PROCEDURE IF EXISTS [dbo].[proc_OUSettingsMany]
            GO";

            Sql(sql);
        }
    }
}
