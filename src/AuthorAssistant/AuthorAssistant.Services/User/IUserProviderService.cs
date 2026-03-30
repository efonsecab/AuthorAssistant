using System;
using System.Collections.Generic;
using System.Text;

namespace AuthorAssistant.Services.User
{
    public interface IUserProviderService
    {
        Task<string?> GetUserIdAsync(CancellationToken cancellationToken);
    }
}
