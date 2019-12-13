CREATE TABLE [dbo].[SEReconcile2] (
    [Name]                NVARCHAR (100) NULL,
    [LocationID]          VARCHAR (50)   NOT NULL,
    [StandardizedAddress] VARCHAR (30)   NULL,
    [City]                VARCHAR (13)   NULL,
    [State]               VARCHAR (2)    NOT NULL,
    [ZipCode]             VARCHAR (5)    NULL,
    [Lat]                 FLOAT (53)     NULL,
    [Lon]                 FLOAT (53)     NULL
);

