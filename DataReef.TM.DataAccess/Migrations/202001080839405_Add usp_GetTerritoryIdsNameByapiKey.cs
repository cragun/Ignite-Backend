namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Addusp_GetTerritoryIdsNameByapiKey : DbMigration
    {
        public override void Up()
        {
            var sql = @"Create PROCEDURE [dbo].[usp_GetTerritoryIdsNameByapiKey]
(
	@latitude FLOAT,
	@longitude FLOAT ,
	@apiKey varchar(100)= NULL,
	@ouid uniqueidentifier = NULL
)
AS
BEGIN

	-- exec usp_GetTerritoryIdsNameByapiKey 29.973433, -95.243265, '1f82605d3fe666478f3f4f1ee25ae828', '48a930a3-4ad0-4786-a480-5585c2b2f117'
	--exec usp_GetTerritoryIdsNameByapiKey 29.920071,	-95.498855,NULL,'1E9E5809-45F2-4CEE-AACB-6617DD232A40'
    --exec usp_GetTerritoryIdsNameByapiKey 0,0,NULL,'1E9E5809-45F2-4CEE-AACB-6617DD232A40'
	DECLARE @OUIds TABLE ([Guid] uniqueidentifier)
	--get child OUs using Key
		;WITH OUTree ([Guid]) AS ( SELECT OUs.Guid FROM OUs WHERE OUs.Guid in (select OUID from dbo.ousettings where value like '%' + @apiKey + '%' and Name = 'Integrations.Options.Selected') and OUs.IsDeleted = 0
		                      UNION ALL
							  SELECT OUs.Guid FROM OUs INNER JOIN OUTree AS T ON OUs.ParentID = T.Guid WHERE OUs.IsDeleted = 0)
        insert into @OUIds select Guid from OUTree
     --get child OUs using Key
	 if(@ouid IS NOT NULL) begin insert into @OUIds select @ouid as Guid end


	 if(@latitude = 0 and @longitude = 0)
	 begin
	 
	 select [Guid] as TerritoryId,Name from Territories where ouid = @ouid order by Name
	 return
	 end


	DECLARE @id uniqueidentifier
	DECLARE @Name varchar(100)
	DECLARE @WellKnownText nvarchar(max)
	DECLARE @shape geography
	DECLARE @point geography = geography::Point(@latitude, @longitude, 4326)
	DECLARE @territoryIds TABLE ([Guid] uniqueidentifier,Name varchar(100))

	DECLARE territories_cursor CURSOR FOR  
	SELECT  [Guid],Name, WellKnownText FROM Territories where ouid in (select Guid from @OUIds)

	OPEN territories_cursor   
	FETCH NEXT FROM territories_cursor INTO @id,@Name, @WellKnownText 

	WHILE @@FETCH_STATUS = 0   
	BEGIN   
	
		set @shape = geography::STGeomFromText(@WellKnownText, 4326).MakeValid()
		select @shape = case when @shape.EnvelopeAngle() >= 90 THEN @shape.ReorientObject() else @shape end

		IF @shape.STContains(@point) = 1 INSERT INTO @territoryIds VALUES (@id,@Name)
    
		FETCH NEXT FROM territories_cursor INTO @id,@Name, @WellKnownText  
	END   

	CLOSE territories_cursor   
	DEALLOCATE territories_cursor

	select [Guid] as TerritoryId,Name from @territoryIds order by Name
END";

            Sql(sql);
        }

        public override void Down()
        {
            var sql = @"
            DROP PROCEDURE IF EXISTS [dbo].[usp_GetTerritoryIdsNameByapiKey]
            GO";

            Sql(sql);
        }
    }
}
