CREATE TYPE [dbo].[BatchPrescreenInputTableType] AS TABLE (
    [Id]          UNIQUEIDENTIFIER NOT NULL,
    [HouseNumber] NVARCHAR (50)    NULL,
    [StreetName]  NVARCHAR (50)    NULL,
    [ZipCode]     CHAR (9)         NULL);

