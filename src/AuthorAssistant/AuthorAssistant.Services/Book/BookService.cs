using AuthorAssistant.DataAccess.Data;
using AuthorAssistant.DataAccess.Models;
using AuthorAssistant.Models.Book;
using Microsoft.EntityFrameworkCore;

namespace AuthorAssistant.Services.Book
{
    public class BookService(AuthorAssistantDatabaseContext authorAssistantDatabaseContext)
    {
        public async Task<BookModel> CreateBookAsync(CreateBookModel bookModel, CancellationToken cancellationToken)
        {
            var entity = await authorAssistantDatabaseContext.Books
                .SingleOrDefaultAsync(p => p.Name == bookModel.Name && p.OwnerId == bookModel.OwnerId,
                cancellationToken: cancellationToken);
            if (entity is null)
            {
                entity = new DataAccess.Models.Book()
                {
                    Description = bookModel.Description,
                    Name = bookModel.Name,
                    OwnerId = bookModel.OwnerId
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
    }
}
