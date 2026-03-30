using AuthorAssistant.DataAccess.Data;
using AuthorAssistant.DataAccess.Models;
using AuthorAssistant.Models.Book;
using AuthorAssistant.Services.Enums;
using AuthorAssistant.Services.NanoBanana;
using AuthorAssistant.Services.NanoBanana.Enums;
using Microsoft.EntityFrameworkCore;

namespace AuthorAssistant.Services.Book
{
    public class BookService(AuthorAssistantDatabaseContext authorAssistantDatabaseContext,
        INanoBananaService nanoBananaService)
    {
        public async Task<BookModel> CreateBookAsync(CreateBookModel bookModel, string ownerId, CancellationToken cancellationToken)
        {
            var entity = await authorAssistantDatabaseContext.Books
                .SingleOrDefaultAsync(p => p.Name == bookModel.Name && p.OwnerId == ownerId,
                cancellationToken: cancellationToken);
            if (entity is null)
            {
                entity = new DataAccess.Models.Book()
                {
                    Description = bookModel.Description,
                    Name = bookModel.Name,
                    OwnerId = ownerId,
                    TextContent = bookModel.TextContent
                };
                await authorAssistantDatabaseContext.Books.AddAsync(entity, cancellationToken: cancellationToken);
                await authorAssistantDatabaseContext.SaveChangesAsync(cancellationToken);
                BookModel result = new BookModel()
                {
                    Description = entity.Description,
                    Name = entity.Name,
                    OwnerId = entity.OwnerId,
                    BookId = entity.BookId,
                    TextContent = entity.TextContent
                };
                return result;
            }
            else
            {
                throw new Exception("A book with the same name already exists for the given user");
            }
        }

        public async Task<BookModel> UpdateBookAsync(long bookId, CreateBookModel createBookModel, string userId, CancellationToken cancellationToken)
        {
            //Validate there are no other books with the same name for the user
            var entityWithSameName = await authorAssistantDatabaseContext.Books
                .SingleOrDefaultAsync(p => p.Name == createBookModel.Name && p.OwnerId == userId && p.BookId != bookId,
                cancellationToken: cancellationToken);

            if (entityWithSameName is not null)
            {
                throw new Exception("A book with the same name already exists for the given user");
            }

            //Validate that the book exists and belongs to the user
            var entity = await authorAssistantDatabaseContext.Books
                .SingleOrDefaultAsync(p => p.BookId == bookId && p.OwnerId == userId,
                cancellationToken: cancellationToken);

            if (entity is not null)
            {
                entity.Name = createBookModel.Name;
                entity.Description = createBookModel.Description;
                await authorAssistantDatabaseContext.SaveChangesAsync(cancellationToken);
                return new BookModel()
                {
                    Description = entity.Description,
                    Name = entity.Name,
                    OwnerId = entity.OwnerId,
                    BookId = entity.BookId,
                    TextContent = entity.TextContent
                };
            }
            else
            {
                throw new Exception("Book not found");
            }
        }

        public async Task<BookModel[]?> GetAllBooksForUserAsync(string userId, CancellationToken cancellationToken)
        {
            var entities = await authorAssistantDatabaseContext.Books
                .Where(p => p.OwnerId == userId)
                .ToArrayAsync(cancellationToken: cancellationToken);
            if (entities is not null)
            {
                return entities.Select(entity => new BookModel()
                {
                    Description = entity.Description,
                    Name = entity.Name,
                    OwnerId = entity.OwnerId,
                    BookId = entity.BookId,
                    TextContent = entity.TextContent
                }).ToArray();
            }
            else
            {
                return null;
            }
        }

        public async Task<BookModel> GetBookByBookIdAsync(long bookId, CancellationToken cancellationToken)
        {
            var entity = await authorAssistantDatabaseContext.Books
                .SingleOrDefaultAsync(p => p.BookId == bookId, cancellationToken: cancellationToken);
            if (entity is not null)
            {
                return new BookModel()
                {
                    Description = entity.Description,
                    Name = entity.Name,
                    OwnerId = entity.OwnerId,
                    BookId = entity.BookId,
                    TextContent = entity.TextContent
                };
            }
            else
            {
                throw new Exception("Book not found");
            }
        }

        public async Task<BookCoverImageConceptModel> CreateBookCoverImageConceptAsync(long bookId,
            ImageStyle imageStyle, ImageSize imageSize,
            CancellationToken cancellationToken)
        {
            var entity = await authorAssistantDatabaseContext.Books
                .SingleOrDefaultAsync(p => p.BookId == bookId, cancellationToken: cancellationToken);
            if (entity is not null)
            {
                string prompt = $"Create an book cover image concept for a book with the following details:\n" +
                $"Image Style: {imageStyle}\n" +
                $"Book Title: {entity.Name}\n" +
                $"Book Content: {entity.TextContent}\n" +
                $"Image must not have text\n" +
                $"Image must follow amazon kdp guidelines.\n" +
                $"I will use the image to place in the Amazon KDP Cover Designer";
                var response = await nanoBananaService.CreateImageAsync(prompt, imageSize, cancellationToken);
                BookCoverImageConcept bookCoverImageConcept = new BookCoverImageConcept()
                {
                    BinaryData = response.imageBytes,
                    MimeType = response.mimeType,
                    BookId = bookId
                };
                await authorAssistantDatabaseContext.BookCoverImageConcepts.AddAsync(bookCoverImageConcept, cancellationToken: cancellationToken);
                await authorAssistantDatabaseContext.SaveChangesAsync(cancellationToken);
                BookCoverImageConceptModel bookCoverImageConceptModel = new BookCoverImageConceptModel()
                {
                    BookCoverImageConceptId = bookCoverImageConcept.BookCoverImageConceptId
                };
                return bookCoverImageConceptModel;
            }
            else
            {
                throw new Exception("Book not found");
            }
        }

        public async Task<BookCoverImageConceptModel> GetBookCoverImageConceptByBookCoverImageConceptIdAsync(long bookCoverImageConceptId, CancellationToken cancellationToken)
        {
                        var entity = await authorAssistantDatabaseContext.BookCoverImageConcepts
                .SingleOrDefaultAsync(p => p.BookCoverImageConceptId == bookCoverImageConceptId, cancellationToken: cancellationToken);
            if (entity is not null)
            {
                return new BookCoverImageConceptModel()
                {
                    BookCoverImageConceptId = entity.BookCoverImageConceptId,
                    BinaryData = entity.BinaryData,
                    MimeType = entity.MimeType
                };
            }
            else
            {
                throw new Exception("Book cover image concept not found");
            }
        }

        public async Task UploadBookFileAsync(long bookId, UploadBookFileModel uploadBookFileModel, CancellationToken cancellationToken)
        {
            var entity = await authorAssistantDatabaseContext.Books
                .SingleOrDefaultAsync(p => p.BookId == bookId, cancellationToken: cancellationToken);
            if (entity is not null)
            {
                var bookFileEntity = await authorAssistantDatabaseContext.BookFiles
                    .SingleOrDefaultAsync(p => p.BookId == bookId, cancellationToken: cancellationToken);
                if (bookFileEntity is not null)
                {
                    bookFileEntity.BinaryData = uploadBookFileModel.BinaryData;
                    bookFileEntity.MimeType = uploadBookFileModel.MimeType;
                }
                else
                {
                    bookFileEntity = new BookFile()
                    {
                        BookId = bookId,
                        BinaryData = uploadBookFileModel.BinaryData,
                        MimeType = uploadBookFileModel.MimeType
                    };
                    await authorAssistantDatabaseContext.BookFiles.AddAsync(bookFileEntity, cancellationToken: cancellationToken);
                }
                await authorAssistantDatabaseContext.SaveChangesAsync(cancellationToken);
            }
            else
            {
                throw new Exception("Book not found");
            }
        }

        public async Task<BookFileModel> GetBookFileByBookIdAsync(long bookId, CancellationToken cancellationToken)
        {
            var entity = await authorAssistantDatabaseContext.BookFiles
                .SingleOrDefaultAsync(p => p.BookId == bookId, cancellationToken: cancellationToken);
            if (entity is not null)
            {
                return new BookFileModel()
                {
                    BookId = entity.BookId,
                    MimeType = entity.MimeType,
                    BinaryData = entity.BinaryData
                };
            }
            else
            {
                throw new Exception("Book file not found");
            }
        }
    }
}