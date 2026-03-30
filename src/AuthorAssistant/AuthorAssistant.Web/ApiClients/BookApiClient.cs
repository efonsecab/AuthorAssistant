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
    }
}
