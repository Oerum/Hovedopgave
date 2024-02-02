namespace Crosscutting;

public class GrantLicenseDto
{
    public string? MinutesToExtend { get; set; } = string.Empty;
    public string? DiscordId { get; set; } = string.Empty;
    public string? DiscordUsername { get; set; } = string.Empty;
    public WhichSpec? Product { get; set; } = 0;
    public string? Hwid { get; set; } = string.Empty;
}