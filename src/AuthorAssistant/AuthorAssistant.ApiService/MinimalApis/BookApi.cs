using AuthorAssistant.ApiService.MinimalApis.Enums;
using AuthorAssistant.Models.Book;
using AuthorAssistant.Services.Book;
using AuthorAssistant.Services.NanoBanana;
using AuthorAssistant.Services.NanoBanana.Enums;
using AuthorAssistant.Services.User;
using AuthorAssistant.Services.Veo;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace AuthorAssistant.ApiService.MinimalApis
{
    public static class BookApi
    {
        public static WebApplication MapBookApi(this WebApplication app)
        {
            var apiGroup = app.MapGroup("/api");
            var bookGroup = apiGroup
                .MapGroup("/book")
                .WithTags("Book");

            bookGroup.MapPost("/createAmazonKdpCoverImageConcept",
                async ([FromServices] INanoBananaService nanoBananaService,
                CoverImageConceptRequest coverImageConceptRequest,
                CancellationToken cancellationToken) =>
            {
                string prompt = $"Create an amazon kdp book cover image concept for a book with the following details:\n" +
                $"Image Style: {coverImageConceptRequest.ImageStyle}\n" +
                $"Book Title: {coverImageConceptRequest.BookTitle}\n" +
                $"Book Content: {coverImageConceptRequest.BookContent}\n" +
                $"Image must not have text\n" +
                $"Image must follow amazon kdp guidelines.\n" +
                $"I will use the image to place in the Amazon KDP Cover Designer";
                var result = await nanoBananaService.CreateImageAsync(prompt, ImageSize.OneK, cancellationToken);
                return result.imageBytes is not null ?
                        Results.File(
                            fileContents: result.imageBytes,
                            contentType: result.mimeType ?? "application/octet-stream")
                        : Results.NoContent();
            });
            bookGroup.MapPost("/uploadBook", async (IFormFile formFile) =>
            {
                if (formFile is null || formFile.Length == 0)
                {
                    return Results.BadRequest("No file uploaded.");
                }
                using MemoryStream memoryStream = new MemoryStream();
                await formFile.CopyToAsync(memoryStream);
                var fileBytes = memoryStream.ToArray();
                return Results.NoContent();
            }).WithName("UploadBook");

            bookGroup.MapPost("/createBook",
                async (
                    [FromServices] BookService bookService,
                    [FromServices] IUserProviderService userProviderService,
                    [FromBody] CreateBookModel createBookModel,
                    CancellationToken cancellationToken) =>
                {
                    var userId = await userProviderService.GetUserIdAsync(cancellationToken);
                    createBookModel.OwnerId = userId;
                    var result = await bookService.CreateBookAsync(createBookModel, cancellationToken);
                    return Results.Ok(result);
                }).WithName("CreateBook");

            bookGroup.MapPost("/createBookPromoVideo",
                async ([FromServices] IVeoService veoService,
                [FromBody] BookVideoRequest request, CancellationToken cancellationToken) =>
            {
                try
                {
                    string prompt = "Create a promotional video for a book with the following details:\n" +
                    $"Show Text In Video: {request.ShowTextInVideo}\n" +
                    $"Add Voice Over: {request.AddVoiceOver}\n" +
                    $"Video Style: {request.VideoStyle}\n" +
                    $"Title: {request.Title}\n" +
                    $"Content: {request.Content}\n";
                    var result = await veoService.CreateVideoAsync(prompt, cancellationToken);
                    return result.videoBytes is not null ?
                    Results.File(
                        fileContents: result.videoBytes,
                        contentType: result.mimeType ?? "application/octet-stream")
                    : Results.NoContent();
                }
                catch (Exception ex)
                {
                    return Results.Problem(detail: ex.Message, statusCode: 500);
                }
            });
            return app;
        }
    }

    public class BookVideoRequest
    {
        [Required]
        public required string? Title { get; set; }
        [Required]
        public required string? Content { get; set; }
        [Required]
        public required bool? ShowTextInVideo { get; set; }
        [Required]
        public required bool? AddVoiceOver { get; set; }
        [Required]
        public required VideoStyle? VideoStyle { get; set; }
    }

    public class CoverImageConceptRequest
    {
        [Required]
        public required string? BookTitle { get; set; }
        [Required]
        public required string? BookContent { get; set; }
        [Required]
        public required ImageStyle? ImageStyle { get; set; }
    }
}