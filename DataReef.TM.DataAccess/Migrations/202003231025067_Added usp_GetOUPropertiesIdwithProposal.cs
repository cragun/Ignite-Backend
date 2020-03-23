namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Addedusp_GetOUPropertiesIdwithProposal : DbMigration
    {
        public override void Up()
        {
            var sql = @"create PROCEDURE [dbo].[usp_GetOUPropertiesIdwithProposal]
(
	@ouid uniqueidentifier = NULL,
	@NameSearch varchar(100) = NULL,
	@skipCnt int = 0,
	@pageSize int = 0
)
AS
BEGIN
-- exec [dbo].[usp_GetOUPropertiesIdwithProposal] '21070753-6ED9-4647-9557-2A21D61E45C7', NULL, 0,20
 if(@NameSearch IS NOT NULL and @NameSearch != '')
 begin


select p.guid  from solar.Proposals as sp inner join properties as p on sp.propertyid = p.guid inner join territories as t on p.territoryid = t.guid
  where t.ouid in (select guid from dbo.outree (@ouid)) and p.Isdeleted = 0 and sp.Isdeleted = 0  and (p.Name like '%' + @NameSearch +'%')
  order by sp.Name  OFFSET (@skipCnt) ROWS FETCH NEXT (@pageSize) ROWS ONLY 
   return
 end

select p.guid  from solar.Proposals as sp inner join properties as p on sp.propertyid = p.guid inner join territories as t on p.territoryid = t.guid
  where t.ouid in (select guid from dbo.outree (@ouid)) and p.Isdeleted = 0 and sp.Isdeleted = 0  
  order by sp.Name  OFFSET (@skipCnt) ROWS FETCH NEXT (@pageSize) ROWS ONLY  
END";

            Sql(sql);
        }

        public override void Down()
        {
            var sql = @"
            DROP PROCEDURE IF EXISTS [dbo].[usp_GetOUPropertiesIdwithProposal]
            GO";

            Sql(sql);
        }
    }
}
