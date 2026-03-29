namespace AuthorAssistant.Services.GoogleGemini
{
    public interface IGoogleGeminiService
    {
        Task<string?> GenerateContentAsync(string prompt, CancellationToken cancellationToken = default);
    }
}
