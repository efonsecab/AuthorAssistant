CREATE TABLE [dbo].[BookPromoVideo]
(
	[BookPromoVideoId] BIGINT NOT NULL CONSTRAINT PK_BookPromoVideo PRIMARY KEY IDENTITY, 
    [BookId] BIGINT NOT NULL, 
    [BinaryData] VARBINARY(MAX) NOT NULL, 
    [MimeType] NVARCHAR(250) NOT NULL, 
    CONSTRAINT [FK_BookPromoVideo_Book] FOREIGN KEY ([BookId]) REFERENCES [Book]([BookId])
)
