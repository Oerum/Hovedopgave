using System.Runtime.CompilerServices;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;

namespace Crosscutting.Configuration.AuthPolicyConfiguration;

public static class PolicyClaimAuth
{
    public static IEnumerable<Claim> ClaimsConfiguration(IConfiguration configuration, DiscordModelDto? model = null, string? hwid = null)
    {
        string adminRoleId = configuration["Discord:Role:Admin"] ?? string.Empty;
        string staffRoleId = configuration["Discord:Role:Staff"] ?? string.Empty;
        string boosterId = configuration["Discord:Role:Boost"] ?? string.Empty;

        var claims = new List<Claim>
        {
            new("user", "user"),
        };

        if (model != null)
        {
            var roles = new HashSet<string>(model.Roles!);

            if (roles.Contains(adminRoleId) || roles.Contains("Mod"))
            {
                claims.Add(new Claim("admin", "admin"));
            }

            if (roles.Contains(staffRoleId) || roles.Contains("Staff"))
            {
                claims.Add(new Claim("staff", "staff"));
            }

            if (roles.Contains(boosterId) || roles.Contains("Server Booster"))
            {
                claims.Add(new Claim("booster", "booster"));
            }
        }

        if (!string.IsNullOrEmpty(hwid))
        {
            claims.Add(new Claim("hwid", "hwid"));
        }

        return claims;
    }
}