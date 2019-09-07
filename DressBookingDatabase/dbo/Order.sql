CREATE TABLE [dbo].[Order]
(
	[Id] BIGINT NOT NULL PRIMARY KEY, 
    [DressId] BIGINT NOT NULL, 
    [UserName] [nvarchar](256) NULL, 
    [StartDate] DATETIME NOT NULL, 
    [EndDate] DATETIME NULL, 
	[Sum] MONEY NOT NULL, 
    [Booking] BIT NOT NULL DEFAULT 0, 
    CONSTRAINT [FK_Order_DressId] FOREIGN KEY ([DressId]) REFERENCES [Dresses]([Id])
)
