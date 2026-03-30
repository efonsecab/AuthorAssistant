using System.ComponentModel.DataAnnotations;

namespace AuthorAssistant.Models.Book
{
    public class CreateBookModel
    {
        [Required]
        [StringLength(maximumLength:50)]
        public required string? Name { get; set; }
        [Required]
        [StringLength(maximumLength:450)]
        public required string? Description { get; set; }
    }
}
