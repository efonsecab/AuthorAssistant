CREATE TABLE [dbo].[BookFile]
(
	[BookFileId] BIGINT NOT NULL PRIMARY KEY IDENTITY, 
    [BookId] BIGINT NOT NULL, 
    [BinaryData] VARBINARY(MAX) NOT NULL, 
    CONSTRAINT [FK_BookFile_Book] FOREIGN KEY ([BookId]) REFERENCES [Book]([BookId])
)
