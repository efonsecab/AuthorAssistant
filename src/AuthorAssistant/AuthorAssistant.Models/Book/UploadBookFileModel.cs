using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AuthorAssistant.Models.Book
{
    public class UploadBookFileModel
    {
        [Required]
        public long? BookId { get; set; }
        [Required]
        public string? MimeType { get; set; }
        [Required]
        public byte[]? BinaryData { get; set; }
    }
}
