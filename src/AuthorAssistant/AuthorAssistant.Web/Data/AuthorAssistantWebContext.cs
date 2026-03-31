using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using AuthorAssistant.Web.Data;

namespace AuthorAssistant.Web.Data
{
    public class AuthorAssistantWebContext(DbContextOptions<AuthorAssistantWebContext> options) : IdentityDbContext<AuthorAssistantWebUser>(options)
    {
    }
}
