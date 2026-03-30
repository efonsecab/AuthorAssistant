using AuthorAssistant.DataAccess.Models;
using AuthorAssistant.Models.Book;
using AuthorAssistant.Services.Book;
using AuthorAssistant.Services.Enums;
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

            bookGroup.MapPost("/uploadBookFile",
                async (
                    [FromServices] BookService bookService,
                    [FromBody] UploadBookFileModel uploadBookFileModel) =>
            {
                await bookService.UploadBookFileAsync(uploadBookFileModel.BookId!.Value, uploadBookFileModel, CancellationToken.None);
            }).WithName("UploadBook");

            bookGroup.MapPost("/createBookCoverImageConcept",
                async ([FromServices] BookService bookService, 
                [FromQuery] long bookId,
                [FromQuery] ImageStyle imageStyle,
                [FromQuery] ImageSize imageSize,
                CancellationToken cancellationToken) =>
                {
                    await bookService.CreateBookCoverImageConceptAsync(bookId, imageStyle, imageSize, cancellationToken);
                    return Results.NoContent();
                });

            bookGroup.MapGet("/bookCoverImageConcept", 
                async ([FromServices] BookService bookService, 
                long bookCoverImageConceptId,
                CancellationToken cancellationToken ) => 
                {
                    var result = await bookService.GetBookCoverImageConceptByBookCoverImageConceptIdAsync(bookCoverImageConceptId, cancellationToken);
                    return result.BinaryData is not null ?
                        Results.File(
                            fileContents: result.BinaryData,
                            contentType: result.MimeType ?? "application/octet-stream")
                        : Results.NoContent();
                });

            bookGroup.MapGet("/content",
                async ([FromServices] BookService bookService,
                       [FromQuery] long bookId,
                       CancellationToken cancellationToken) =>
                {
                    var result = await bookService.GetBookFileByBookIdAsync(bookId, cancellationToken);
                    return result.BinaryData is not null ?
                        Results.File(
                            fileContents: result.BinaryData,
                            contentType: result.MimeType ?? "application/octet-stream")
                        : Results.NoContent();
                }).WithName("GetBookFileContent");

            bookGroup.MapPost("/createBook",
                async (
                    [FromServices] BookService bookService,
                    [FromServices] IUserProviderService userProviderService,
                    [FromBody] CreateBookModel createBookModel,
                    CancellationToken cancellationToken) =>
                {
                    var userId = await userProviderService.GetUserIdAsync(cancellationToken) ?? throw new Exception("User not found");
                    var result = await bookService.CreateBookAsync(createBookModel, userId, cancellationToken);
                    return Results.Ok(result);
                }).WithName("CreateBook");

            bookGroup.MapGet("getAllMyBooks",
                async ([FromServices] BookService bookService,
                       [FromServices] IUserProviderService userProviderService,
                       CancellationToken cancellationToken) =>
                {
                    var userId = await userProviderService.GetUserIdAsync(cancellationToken) ?? throw new Exception("User not found");
                    var result = await bookService.GetAllBooksForUserAsync(userId, cancellationToken);
                    return Results.Ok(result);
                }).WithName("GetAllMyBooks");

            bookGroup.MapGet("/getBookByBookId",
                async ([FromServices] BookService bookService,
                       [FromServices] IUserProviderService userProviderService,
                       [FromQuery] long bookId,
                       CancellationToken cancellationToken) =>
                {
                    var result = await bookService.GetBookByBookIdAsync(bookId, cancellationToken);
                    return Results.Ok(result);
                }).WithName("GetBookByBookId");

            bookGroup.MapPut("/updateBook",
                async ([FromServices] BookService bookService,
                       [FromServices] IUserProviderService userProviderService,
                       [FromBody] CreateBookModel createBookModel,
                       [FromQuery] long bookId,
                       CancellationToken cancellationToken) =>
                {
                    var userId = await userProviderService.GetUserIdAsync(cancellationToken) ?? throw new Exception("User not found");
                    var result = await bookService.UpdateBookAsync(bookId, createBookModel, userId, cancellationToken);
                    return Results.Ok(result);
                }).WithName("UpdateBook");

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