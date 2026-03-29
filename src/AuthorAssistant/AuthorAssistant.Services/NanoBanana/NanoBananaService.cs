
using AuthorAssistant.Services.NanoBanana.Enums;
using Google.GenAI.Types;
using Microsoft.Extensions.Logging;

namespace AuthorAssistant.Services.NanoBanana
{
    public class NanoBananaService(ILogger<NanoBananaService> logger,
        Google.GenAI.Client genAiClient) : INanoBananaService
    {
        private const string generateImageModel = "gemini-3.1-flash-image-preview";
        public async Task<(byte[]? imageBytes, string? mimeType)>
            CreateImageAsync(string prompt, ImageSize imageSize, CancellationToken cancellationToken)
        {
            var contents = new List<Content>
                {
                    new Content
                    {
                        Role = "user",
                        Parts = new List<Part>
                        {
                            new Part { Text = prompt },
                        }
                    },
                };



            var config = new GenerateContentConfig
            {
                ThinkingConfig = new ThinkingConfig
                {
                    ThinkingLevel = "MINIMAL"
                },
                ImageConfig = new ImageConfig
                {
                    AspectRatio = "1:1",
                    ImageSize = imageSize switch
                    {
                        ImageSize.OneK => "1K",
                        ImageSize.TwoK => "2K",
                        ImageSize.FourK => "4K",
                        _ => "1K"
                    }
                },
                ResponseModalities = new List<string>
                    {
                        "IMAGE",
                        "TEXT"
                    },
            };
            var response = await genAiClient.Models.GenerateContentAsync(model: generateImageModel,
                contents: contents, config: config, cancellationToken: cancellationToken);
            var part = response.Parts!.Where(p => p.InlineData?.Data != null)?.FirstOrDefault();
            if (part is null)
            {
                logger.LogWarning("No image data found in the response from Google Gemini: {ResponseText}.", response.Text);
                throw new InvalidOperationException("No image data found in the response from Google Gemini.");
            }
            return (part?.InlineData?.Data, part?.InlineData?.MimeType);
        }
    }
}
