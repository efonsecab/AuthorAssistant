using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AuthorAssistant.Models.Book
{
    public class BookPromoVideoModel
    {
        [Required]
        public long? BookPromoVideoId { get; set; }
        [Required]
        public byte[]? BinaryData { get; set; }
        [Required]
        public string? MimeType { get; set; }
    }
}
