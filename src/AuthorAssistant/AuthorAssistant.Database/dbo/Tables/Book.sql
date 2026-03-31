CREATE TABLE [dbo].[Book]
(
	[BookId] BIGINT NOT NULL CONSTRAINT PK_Book PRIMARY KEY IDENTITY,
    [OwnerId] NVARCHAR(450) NOT NULL,
    [Name] NVARCHAR(50) NOT NULL, 
    [Description] NVARCHAR(4000) NOT NULL, 
    [TextContent] NVARCHAR(MAX) NOT NULL, 
    CONSTRAINT [FK_Book_AspNetUsers] FOREIGN KEY ([OwnerId]) REFERENCES [AspNetUsers]([Id])
)

GO

CREATE UNIQUE INDEX [UI_Book_Name_OwnerId] ON [dbo].[Book] ([Name], [OwnerId])
