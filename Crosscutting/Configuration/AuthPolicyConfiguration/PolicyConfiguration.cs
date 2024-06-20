using Microsoft.Extensions.DependencyInjection;

namespace Crosscutting.Configuration.AuthPolicyConfiguration;

public static class PolicyConfiguration
{
    public const string UserPolicy = "user";
    public const string StaffPolicy = "staff";
    public const string AdminPolicy = "admin";
    public const string BoosterPolicy = "booster";
    public const string AdminOrStaff = "AdminOrStaff";
    public const string AuthUser = "AuthUser";

    public static IServiceCollection AddClaimPolicyConfiguration(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy("user", policy => policy.RequireClaim("user"));
            options.AddPolicy("staff", policy => policy.RequireClaim("staff"));
            options.AddPolicy("admin", policy => policy.RequireClaim("admin"));
            options.AddPolicy("booster", policy => policy.RequireClaim("booster"));
            options.AddPolicy("AdminOrStaff", policy => policy.RequireAssertion(context => context.User.HasClaim(c => c.Value == "admin" || c.Value == "staff")));
            options.AddPolicy("AuthUser", policy => policy.RequireAssertion(context => context.User.HasClaim(c => c.Value == "user" || c.Value == "hwid")));
        });

        return services;
    }
}

