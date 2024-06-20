using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Crosscutting.Configuration.AuthPolicyConfiguration;
public static class PolicyClaimAuth
{
    public static IEnumerable<Claim> ClaimsConfiguration(IConfiguration configuration, ILogger logger, DiscordModelDto? model = null,  string? hwid = null)
    {
        try
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
                var roles = new HashSet<string>(model.Roles ?? []);

                if (roles.Contains(adminRoleId) || roles.Contains("[ARTIFACT]"))
                {
                    claims.Add(new Claim("admin", "admin"));
                }

                if (roles.Contains(staffRoleId) || roles.Contains("[LEGENDARY]"))
                {
                    claims.Add(new Claim("staff", "staff"));
                }

                if (roles.Contains(boosterId) || roles.Contains("[TOKEN]"))
                {
                    claims.Add(new Claim("booster", "booster"));
                }
            }

            if (!string.IsNullOrEmpty(hwid))
            {
                claims.Add(new Claim("hwid", "hwid"));
            }

            logger.LogInformation($"HWID: {hwid} User: {model?.DiscordUsername ?? "NONE"} Claims: {string.Join(", ", claims)}");

            return claims;
        }
        catch (Exception ex)
        {
            logger.LogError($"Unsuccessful Claims Configuration: {1}", ex.Message);
            throw;
        }
    }
}