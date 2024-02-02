using Microsoft.Extensions.DependencyInjection;

namespace Crosscutting.Configuration.AuthPolicyConfiguration;

public static class PolicyConfiguration
{
    public const string UserPolicy = "user";
    public const string HwidPolicy = "hwid";
    public const string StaffPolicy = "staff";
    public const string AdminPolicy = "admin";
    public const string BoosterPolicy = "booster";

    public static IServiceCollection AddClaimPolicyConfiguration(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy("user", policy => policy.RequireClaim("user"));
            options.AddPolicy("hwid", policy => policy.RequireClaim("hwid"));
            options.AddPolicy("staff", policy => policy.RequireClaim("staff"));
            options.AddPolicy("admin", policy => policy.RequireClaim("admin"));
            options.AddPolicy("booster", policy => policy.RequireClaim("booster"));
        });

        return services;
    }
}

