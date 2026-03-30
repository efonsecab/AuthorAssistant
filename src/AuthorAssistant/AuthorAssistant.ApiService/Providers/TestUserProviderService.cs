using AuthorAssistant.DataAccess.Data;
using AuthorAssistant.Services.User;
using Microsoft.EntityFrameworkCore;

namespace AuthorAssistant.ApiService.Providers
{
    public class TestUserProviderService(AuthorAssistantDatabaseContext authorAssistantDatabaseContext) : IUserProviderService
    {
        public async Task<string?> GetUserIdAsync(CancellationToken cancellationToken)
        {
            var user = await authorAssistantDatabaseContext.AspNetUsers.FirstOrDefaultAsync();
            if (user is null)
            {
                throw new Exception("No user found in the database.");
            }
            else
            {
                return user.Id;
            }
        }
    }
}
