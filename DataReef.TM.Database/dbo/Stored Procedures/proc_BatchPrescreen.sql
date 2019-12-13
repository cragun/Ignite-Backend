CREATE PROCEDURE [dbo].[proc_BatchPrescreen]
    @BatchPrescreenInput BatchPrescreenInputTableType READONLY,
	@BatchPrescreenTableName nvarchar(MAX)
AS 
BEGIN
	SET NOCOUNT ON;
	
	DECLARE @query as nvarchar(max);
	set @query  =
	N'SELECT input.[Id]
			,data.CreditCategory as [CreditCategory]
			,data."First Name" + '' '' +  data."Last Name" as [Name]
	FROM @BatchPrescreenInput input
	LEFT JOIN [' + @BatchPrescreenTableName + '] data
		ON (
			(input.[HouseNumber] IS NULL OR data."House #" = input.[HouseNumber])
			AND (input.[StreetName] IS NULL OR data."Street Name" = input.[StreetName])
			AND (input.[ZipCode] IS NULL OR data."ZIP Code" = input.[ZipCode])
		)';
		
	EXEC sp_executesql @query, N'@BatchPrescreenInput BatchPrescreenInputTableType READONLY', @BatchPrescreenInput
END;