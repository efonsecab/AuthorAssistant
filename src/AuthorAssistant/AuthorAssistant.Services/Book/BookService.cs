using AuthorAssistant.DataAccess.Data;
using AuthorAssistant.DataAccess.Models;
using AuthorAssistant.Models.Book;
using Microsoft.EntityFrameworkCore;

namespace AuthorAssistant.Services.Book
{
    public class BookService(AuthorAssistantDatabaseContext authorAssistantDatabaseContext)
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
                    OwnerId = ownerId
                };
                await authorAssistantDatabaseContext.Books.AddAsync(entity, cancellationToken: cancellationToken);
                await authorAssistantDatabaseContext.SaveChangesAsync(cancellationToken);
                BookModel result = new BookModel()
                {
                    Description = entity.Description,
                    Name = entity.Name,
                    OwnerId = entity.OwnerId,
                    BookId = entity.BookId
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
                    BookId = entity.BookId
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
                    BookId = entity.BookId
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
                    BookId = entity.BookId
                };
            }
            else
            {
                throw new Exception("Book not found");
            }
        }
    }
}