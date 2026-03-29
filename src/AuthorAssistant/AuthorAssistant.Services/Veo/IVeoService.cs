using System;
using System.Collections.Generic;
using System.Text;

namespace AuthorAssistant.Services.Veo
{
    public interface IVeoService
    {
        Task<(byte[]? videoBytes, string? mimeType)> CreateVideoAsync(string prompt, CancellationToken cancellationToken);
    }
}
