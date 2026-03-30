using System.ComponentModel.DataAnnotations;

namespace AuthorAssistant.Models.Book
{
    public class CreateBookModel
    {
        [Required]
        [StringLength(maximumLength:50)]
        public string? Name { get; set; }
        [Required]
        [StringLength(maximumLength:450)]
        public string? Description { get; set; }
        [Required]
        [StringLength(maximumLength: 125000)]
        public string? TextContent { get; set; }
    }
}
