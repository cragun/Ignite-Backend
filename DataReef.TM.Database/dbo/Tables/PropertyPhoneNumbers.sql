CREATE TABLE [dbo].[PropertyPhoneNumbers] (
    [PropertyID]  VARCHAR (50) NOT NULL,
    [PhoneNumber] VARCHAR (50) NULL,
    CONSTRAINT [PK_PropertyPhoneNumbers] PRIMARY KEY CLUSTERED ([PropertyID] ASC)
);

