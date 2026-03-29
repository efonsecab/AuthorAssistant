using Microsoft.AspNetCore.Mvc;
using AuthorAssistant.Services.GoogleGemini;
using System.ComponentModel.DataAnnotations;
using AuthorAssistant.Services.NanoBanana.Enums;
using AuthorAssistant.Services.NanoBanana;

namespace AuthorAssistant.ApiService.MinimalApis
{
    public static class SubstackApi
    {
        public static WebApplication MapSubstackApi(this WebApplication app)
        {
            app.MapPost("api/substack/generateArticle",
                async ([FromServices] IGoogleGeminiService googleGeminiService,
                [FromBody] SubstackArticleRequest request, CancellationToken cancellationToken) =>
            {
                string prompt = $"I'm writing an article for my Substack publication {request.PublicationName}. " +
                    $"The publication is about {request.PublicationDescription}. " +
                    $"I need you to improve my article based on my current draft. " +
                    $"You are not allowed to removed example. " +
                    $"You are not allowed to synthetize the current content. " +
                    $"You must keep the essence of my original article. " +
                    $"You must use my author's voice. " +
                    $"You can only expand the article not reduce it. " +
                    $"You must also give me a viral title and a viral subtitle. ";
                prompt +=
                    $"Current Title: {request.TitleDraft}. " +
                    $"Current Content: {request.ArticleDraft}";
                var result = await googleGeminiService.GenerateContentAsync(prompt, cancellationToken);
                return result is not null ? Results.Ok(result) : Results.NoContent();
            }).WithName("GenerateArticle");

            app.MapPost("api/substack/generateImage",
                async ([FromServices] INanoBananaService nanoBananaService,
                [FromBody] SubstackImageRequest request, CancellationToken cancellationToken) =>
                {
                    try
                    {
                        string prompt = $"Create an image for an article. " +
                        $"The image style must be: {request.ImageStyle}. " +
                        $"The image should be eye-catching and relevant to the article content. " +
                        $"The image should be in a vertical format suitable for Substack articles. " +
                        $"Publication Name: {request.PublicationName}. " +
                        $"Publication Description: {request.PublicationDescription}. " +
                        $"Article Title: {request.Title}. " +
                        $"Article Content: {request.Article}. ";
                        var result = await nanoBananaService.CreateImageAsync(prompt,
                            imageSize: request.Generate4KImage == true ? ImageSize.FourK: ImageSize.OneK,
                            cancellationToken);
                        return result.imageBytes is not null ?
                        Results.File(
                            fileContents: result.imageBytes,
                            contentType: result.mimeType ?? "application/octet-stream")
                        : Results.NoContent();
                    }
                    catch (Exception ex)
                    {
                        return Results.Problem(detail: ex.Message, statusCode: 500);
                    }
                }).WithName("GenerateImage");

            return app;
        }
    }

    public class SubstackArticleRequest
    {
        [Required]
        public required string? TitleDraft { get; set; }
        [Required]
        public required string? ArticleDraft { get; set; }
        [Required]
        public required string? PublicationName { get; set; }
        [Required]
        public required string? PublicationDescription { get; set; }
    }

    public class SubstackImageRequest
    {
        [Required]
        public required string? Title { get; set; }
        [Required]
        public required string? Article { get; set; }
        [Required]
        public required string? PublicationName { get; set; }
        [Required]
        public required string? PublicationDescription { get; set; }
        [Required]
        public required bool? Generate4KImage { get; set; }
        [Required]
        public required ImageStyle? ImageStyle { get; set; }
    }

    public enum ImageStyle
    {
        Photorealistic,
        Animated,
        LEGO,
        Anime,
        Animated_Transformers,
        LiveAction_Transformers,
        PixelArt
    }
}
