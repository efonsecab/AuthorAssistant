using System.ComponentModel.DataAnnotations;

namespace AuthorAssistant.Models.Book
{
    public class BookFileModel
    {
        [Required]
        public required long? BookId { get; set; }
        [Required]
        public required string? MimeType { get; set; }
        [Required]
        public required byte[]? BinaryData { get; set; }
    }
}
