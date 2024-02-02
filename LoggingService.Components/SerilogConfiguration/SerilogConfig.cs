using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Json;

namespace LoggingService.Components.SerilogConfiguration;

public static class SerilogConfig
{
    public static ILoggingBuilder AddLoggerConfig(this ILoggingBuilder services, IConfiguration configuration)
    {
        services.ClearProviders();

        var loggerConfiguration = new LoggerConfiguration()
            .WriteTo.Console(new JsonFormatter())
            .WriteTo.Seq(configuration["Serilog:Seq"] ?? "NONE", LogEventLevel.Information);

        services.AddSerilog(loggerConfiguration.CreateLogger());

        return services;
    }
}
