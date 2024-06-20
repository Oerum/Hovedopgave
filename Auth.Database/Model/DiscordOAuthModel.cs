using System.ComponentModel.DataAnnotations;

namespace Auth.Database.Model
{
    public class DiscordOAuthModel
    {
        [Key]
        public string? State { get; set; }
        public DateTime? Expires_at { get; set; }
        public ulong Id { get; set; }
        public string? Username { get; set; }
        public string? Discriminator { get; set; }
        public string? Global_name { get; set; }
        public string? Avatar { get; set; }
        public bool? Bot { get; set; }
        public bool? System { get; set; }
        public bool? Mfa_enabled { get; set; }
        public string? Banner { get; set; }
        public int? Accent_color { get; set; }
        public string? Locale { get; set; }
        public bool? Verified { get; set; }
        public string? Email { get; set; }
        public int? Flags { get; set; }
        public int? Premium_type { get; set; }
        public int? Public_flags { get; set; }
        public string? Avatar_decoration { get; set; }
    }
}
