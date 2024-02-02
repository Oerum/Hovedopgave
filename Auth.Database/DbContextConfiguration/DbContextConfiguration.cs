using Auth.Database.Model;
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
        //$env:ConnectionStrings__DB__BCr=''

        //Pomelo and EFCore / Extensions must be compatible!

        services.AddDbContext<AuthDbContext>(options =>
        {
            options.UseMySql(builder["ConnectionStrings:DB:BC"] ?? throw new InvalidOperationException("Version Exception"),
                ServerVersion.AutoDetect(builder["ConnectionStrings:DB:BC"]) ?? throw new InvalidOperationException("ConnStr Exception"),
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
            options.UseMySql(builder["ConnectionStrings:DB:BC"] ?? throw new InvalidOperationException("Version Exception"),
                ServerVersion.AutoDetect(builder["ConnectionStrings:DB:BC"]) ?? throw new InvalidOperationException("ConnStr Exception"),
                x =>
                {

                });
        });

        return services;
    }


}