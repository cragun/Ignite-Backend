CREATE TABLE [dbo].[PrescreenRawMatches] (
    [PrescreenID] INT          NULL,
    [FirstName]   VARCHAR (50) NULL,
    [LastName]    VARCHAR (50) NULL,
    [LocationID]  VARCHAR (50) NULL,
    [IsMatch]     BIT          NULL
);


GO
CREATE NONCLUSTERED INDEX [pr_locid]
    ON [dbo].[PrescreenRawMatches]([IsMatch] ASC, [LocationID] ASC);

