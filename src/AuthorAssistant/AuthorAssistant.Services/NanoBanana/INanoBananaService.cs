using AuthorAssistant.Services.NanoBanana.Enums;

namespace AuthorAssistant.Services.NanoBanana
{
    public interface INanoBananaService
    {
        Task<(byte[]? imageBytes, string? mimeType)> CreateImageAsync(string prompt, ImageSize imageSize, CancellationToken cancellationToken);
    }
}
