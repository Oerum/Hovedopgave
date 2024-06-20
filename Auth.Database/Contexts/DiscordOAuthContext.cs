using Auth.Database.Model;
using Microsoft.EntityFrameworkCore;

namespace Auth.Database.Contexts
{
    public class DiscordOAuthContext : DbContext
    {
        public DiscordOAuthContext(DbContextOptions<DiscordOAuthContext> options) : base(options)
        {

        }

        public DbSet<DiscordOAuthModel> OAuth { get; set; }
        
    }
}
