using AuthorAssistant.Services.GoogleGemini.Enums;

namespace AuthorAssistant.Services.GoogleGemini
{
    public interface IGoogleGeminiService
    {
        Task<(byte[]? imageBytes, string? mimeType)> CreateImageAsync(string prompt, ImageSize imageSize, CancellationToken cancellationToken);
        Task<string?> GenerateContentAsync(string prompt, CancellationToken cancellationToken = default);
    }
}
