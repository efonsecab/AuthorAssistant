using AuthorAssistant.ApiService.MinimalApis.Enums;
using AuthorAssistant.Models.Book;
using AuthorAssistant.Services.Book;
using AuthorAssistant.Services.GoogleGemini;
using AuthorAssistant.Services.NanoBanana;
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
            app.MapPost("api/book/CreateBook", 
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
            
            app.MapPost("api/book/createBookPromoVideo",
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
}