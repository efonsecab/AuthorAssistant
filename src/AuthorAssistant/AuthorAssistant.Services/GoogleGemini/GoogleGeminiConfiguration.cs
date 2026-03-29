using System.ComponentModel.DataAnnotations;


namespace AuthorAssistant.Services.GoogleGemini
{
    public class GoogleGeminiConfiguration
    {
        [Required]
        public required string? ApiKey { get; set; }
    }
}
