using System.ComponentModel.DataAnnotations;

namespace Auth.Components
{
    public class DiscordOAuthDTO
    {
        public required string State { get; set; }
    }
}
