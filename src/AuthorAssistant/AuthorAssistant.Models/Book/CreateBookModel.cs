using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AuthorAssistant.Models.Book
{
    public class CreateBookModel
    {
        [Required]
        [StringLength(maximumLength:450)]
        public required string? OwnerId{ get; set; }
        [Required]
        [StringLength(maximumLength:50)]
        public required string? Name { get; set; }
        [Required]
        [StringLength(maximumLength:450)]
        public required string? Description { get; set; }
    }
}
