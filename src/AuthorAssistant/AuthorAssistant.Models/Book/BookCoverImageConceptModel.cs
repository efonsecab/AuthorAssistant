using System.ComponentModel.DataAnnotations;

namespace AuthorAssistant.Models.Book
{
    public class BookCoverImageConceptModel
    {
        [Required]
        public long? BookCoverImageConceptId { get; set; }
        [Required]
        public byte[]? BinaryData { get; set; }
        [Required]
        public string? MimeType { get; set; }
    }
}
