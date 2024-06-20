using Auth.Database.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Auth.Database.DbContextConfiguration;

public static class DbContextConfiguration
{
    public static IServiceCollection AddAuthDbContext(this IServiceCollection services, IConfiguration builder)
    {
        // Add services to the container.
        //dotnet ef migrations add 0.1 --project Auth.Database --startup-project EFCore --context AuthDbContext
        //dotnet ef database update --project Auth.Database --startup-project EFCore --context AuthDbContext
        //dotnet tool update --global dotnet-ef
        //$env:ConnectionStrings__DB__Core='server=localhost;port=3307;database=Core.Master;user=changeme;password=changeme;AllowPublicKeyRetrieval=True;SslMode=preferred;'

        //Pomelo and EFCore / Extensions must be compatible!

        services.AddDbContext<AuthDbContext>(options =>
        {
            options.UseMySql(builder["ConnectionStrings:DB:Core"] ?? throw new InvalidOperationException("Version Exception"),
                ServerVersion.AutoDetect(builder["ConnectionStrings:DB:Core"]) ?? throw new InvalidOperationException("ConnStr Exception"),
                x =>
                {

                });
        });

        return services;
    }

    public static IServiceCollection AddSagaDbContext(this IServiceCollection services, IConfiguration builder)
    {
        services.AddDbContext<DiscordSagaDbContext>(options =>
        {
            options.UseMySql(builder["ConnectionStrings:DB:Core"] ?? throw new InvalidOperationException("Version Exception"),
                ServerVersion.AutoDetect(builder["ConnectionStrings:DB:Core"]) ?? throw new InvalidOperationException("ConnStr Exception"),
                x =>
                {

                });
        });

        return services;
    }

    public static IServiceCollection AddOAuthContext(this IServiceCollection services, IConfiguration builder)
    {
        services.AddDbContext<DiscordOAuthContext>(options =>
        {
            options.UseMySql(builder["ConnectionStrings:DB:Core"] ?? throw new InvalidOperationException("Version Exception"),
                ServerVersion.AutoDetect(builder["ConnectionStrings:DB:Core"]) ?? throw new InvalidOperationException("ConnStr Exception"),
                x =>
                {

                });
        });

        return services;
    }
}