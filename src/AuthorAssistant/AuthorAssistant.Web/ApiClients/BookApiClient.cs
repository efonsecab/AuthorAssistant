using AuthorAssistant.Models.Book;

namespace AuthorAssistant.Web.ApiClients
{
    public class BookApiClient(HttpClient httpClient)
    {
        public async Task<BookModel> CreateBookAsync(CreateBookModel createBookModel, CancellationToken cancellationToken)
        {
            var response = await httpClient.PostAsJsonAsync("/api/book/createBook", createBookModel, cancellationToken);
            response.EnsureSuccessStatusCode();
            var book = await response.Content.ReadFromJsonAsync<BookModel>(cancellationToken: cancellationToken);
            return book!;
        }

        public async Task<BookModel[]?> GetAllMyBooksAsync(CancellationToken cancellationToken)
        {
            var response = await httpClient.GetAsync("/api/book/getAllMyBooks", cancellationToken);
            response.EnsureSuccessStatusCode();
            var books = await response.Content.ReadFromJsonAsync<BookModel[]>(cancellationToken: cancellationToken);
            return books;
        }

        public async Task<BookModel?> GetBookByIdAsync(long bookId, CancellationToken cancellationToken)
        {
            var response = await httpClient.GetAsync($"/api/book/getBookByBookId?bookId={bookId}", cancellationToken);
            response.EnsureSuccessStatusCode();
            var book = await response.Content.ReadFromJsonAsync<BookModel>(cancellationToken: cancellationToken);
            return book;
        }

        public async Task<BookModel> UpdateBookAsync(long bookId, CreateBookModel createBookModel, CancellationToken cancellationToken)
        {
            var response = await httpClient.PutAsJsonAsync($"/api/book/updateBook?bookId={bookId}", createBookModel, cancellationToken);
            response.EnsureSuccessStatusCode();
            var book = await response.Content.ReadFromJsonAsync<BookModel>(cancellationToken: cancellationToken);
            return book!;
        }
    }
}