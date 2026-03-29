using Microsoft.Extensions.Logging;

namespace AuthorAssistant.Services.GoogleGemini
{
    public class GoogleGeminiService(ILogger<GoogleGeminiService> logger,
        Google.GenAI.Client genAiClient) : IGoogleGeminiService
    {
        private const string generateContentModel = "gemini-3.1-flash-lite-preview";
        public async Task<string?> GenerateContentAsync(string prompt, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await genAiClient.Models.GenerateContentAsync(model: generateContentModel,
                    contents: prompt, cancellationToken: cancellationToken);
                return response?.Text;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error generating content with Google Gemini.");
                throw;
            }
        }
    }
}
