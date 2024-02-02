namespace Crosscutting
{
    public class AuthModelDTO
    {
        public string? Email { get; set; }
        public string? Firstname { get; set; }
        public string? Lastname { get; set; }
        public string? DiscordUsername { get; set; }
        public string? DiscordId { get; set; }
        public string? HWID { get; set; }
        //ActiveLicenses
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public string ProductName { get; set; } //ProductName
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public DateTime EndDate { get; set; }
        public string? UserId { get; set; }
        public WhichSpec ProductNameEnum { get; set; } //ProductNameEnum
    }
}
