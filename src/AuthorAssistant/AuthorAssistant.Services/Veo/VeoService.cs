using Google.GenAI;
using Google.GenAI.Types;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthorAssistant.Services.Veo
{
    public class VeoService(ILogger<VeoService> logger, Client genAIClient): IVeoService
    {
        private const string generateVideoModel = "veo-3.1-fast-generate-preview";
        public async Task<(byte[]? videoBytes, string? mimeType)> CreateVideoAsync(string prompt, CancellationToken cancellationToken)
        {
            var source = new GenerateVideosSource
            {
                Prompt = prompt,
            };

            var config = new GenerateVideosConfig
            {
                //PersonGeneration = "dont_allow", // supported values: "dont_allow" or "allow_adult" or "allow_all"
                AspectRatio = "16:9", // supported values: "16:9", etc.
                NumberOfVideos = 1, // supported values: 1 - 4
                DurationSeconds = 8, // supported values: 5 - 8
                Resolution = "720p", // supported values: "720p", "1080p", or "4k"
            };

            var operation = await genAIClient.Models.GenerateVideosAsync(
                model: generateVideoModel,
                source: source,
                config: config
            );

            while (operation.Done != true)
            {
                try
                {
                    await Task.Delay(10000);
                    operation = await genAIClient.Operations.GetAsync(operation, null);
                }
                catch (TaskCanceledException ex)
                {
                    logger.LogError(ex, "Video generation operation was canceled.");
                    break;
                }
            }
            if (operation.Response?.GeneratedVideos?.Count == 0)
            {
                logger.LogError("No videos were generated.");
                throw new Exception("Video generation failed: No videos were generated.");
            }
            var firstVideo = operation.Response!.GeneratedVideos![0];
            var videoStream = await genAIClient.Files.DownloadAsync(firstVideo, cancellationToken: cancellationToken);
            byte[]? videoBytes;
            using (MemoryStream ms = new MemoryStream())
            {
                // Copy the entire contents of the input stream to the MemoryStream
                await videoStream.CopyToAsync(ms, cancellationToken);

                // Return the data as a byte array
                videoBytes = ms.ToArray();
            }
            return (videoBytes, "video/mp4");
        }
    }
}
