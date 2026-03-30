CREATE TABLE [dbo].[BookCoverImageConcept]
(
	[BookCoverImageConceptId] BIGINT NOT NULL CONSTRAINT PK_BookCoverImageConcept PRIMARY KEY IDENTITY, 
    [BookId] BIGINT NOT NULL, 
    [BinaryData] VARBINARY(MAX) NOT NULL, 
    [MimeType] NVARCHAR(250) NOT NULL, 
    CONSTRAINT [FK_BookCoverImageConcept_Book] FOREIGN KEY ([BookId]) REFERENCES [Book]([BookId])
)
