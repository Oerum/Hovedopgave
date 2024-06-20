using BoundBot.Application.AdminCheckLicenses.Implementation;
using BoundBot.Application.AdminCheckLicenses.Interface;
using BoundBot.Application.CheckLicenses.Implementation;
using BoundBot.Application.CheckLicenses.Interface;
using BoundBot.Application.DatabaseBackup.Implementation;
using BoundBot.Application.DatabaseBackup.Interface;
using BoundBot.Application.ExtendLicenses.Implementation;
using BoundBot.Application.ExtendLicenses.Interface;
using BoundBot.Application.GetCoupon.Implementation;
using BoundBot.Application.GetCoupon.Interface;
using BoundBot.Application.Gpt.Implementation;
using BoundBot.Application.Gpt.Interface;
using BoundBot.Application.GrantLicense.Implementation;
using BoundBot.Application.GrantLicense.Interface;
using BoundBot.Application.OnJoinedServerEvent.JoinMessage.Implementation;
using BoundBot.Application.OnJoinedServerEvent.JoinMessage.Interface;
using BoundBot.Application.StaffLicense.Implementation;
using BoundBot.Application.StaffLicense.Interface;
using BoundBot.Application.UpdateDiscord.Implementation;
using BoundBot.Application.UpdateDiscord.Interface;
using BoundBot.Application.UpdateHwid.Implementation;
using BoundBot.Application.UpdateHwid.Interface;
using BoundBot.Connection.DiscordConnectionHandler.DiscordClientLibrary;
using BoundBot.Domain.RoleCheck;
using BoundBot.Components.Members;
using BoundBot.Infrastructure.AdminCheckLicenses;
using BoundBot.Infrastructure.CheckLicenses;
using BoundBot.Infrastructure.DatabaseBackup;
using BoundBot.Infrastructure.ExtendLicenses;
using BoundBot.Infrastructure.GetCoupon;
using BoundBot.Infrastructure.Gpt;
using BoundBot.Infrastructure.GrantLicense;
using BoundBot.Infrastructure.OnJoinedServerEvent;
using BoundBot.Infrastructure.StaffLicense;
using BoundBot.Infrastructure.UpdateDiscord;
using BoundBot.Infrastructure.UpdateHwid;
using BoundBot.Services;
using BoundBot.Services.TextCommandModules;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using LoggingService.Components.SerilogConfiguration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using BoundBot.Application.AlterLicense.Interface;
using BoundBot.Infrastructure.AlterLicense;
using BoundBot.Application.AlterLicense.Implementation;
using System.Reflection.Emit;

namespace BoundBot;

public class DiscordBot
{
    public static async Task Main()
    {
        var configBuilder = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .AddEnvironmentVariables()
            .Build();

        var service = new ServiceCollection();

        service.AddSingleton<IConfiguration>(configBuilder);
        service.AddSingleton<IDiscordConnectionHandler, DiscordConnectionHandler>();

        service.AddHttpClient("httpClient", httpClient =>
        {
            httpClient.BaseAddress = new Uri(configBuilder["HttpClient:connStr"] ?? string.Empty);
        });
        var serviceProvider = service.BuildServiceProvider();

        var connectionHandler = serviceProvider.GetService<IDiscordConnectionHandler>();

        service.Scan(scan => scan
            .FromExecutingAssembly()
            .AddClasses()
            .AsMatchingInterface()
            .WithScopedLifetime());

        service.TryAddScoped<IDomainRoleCheck, DomainRoleCheck>();
        service.TryAddScoped<IUpdateDiscordImplementation, UpdateDiscordImplementation>();
        service.TryAddScoped<IUpdateDiscordRepository, UpdateDiscordRepository>();
        service.TryAddScoped<IUpdateHwidImplementation, UpdateHwidImplementation>();
        service.TryAddScoped<IUpdateHwidRepository, UpdateHwidRepository>();
        service.TryAddScoped<ICheckLicenseImplementation, CheckLicenseImplementation>();
        service.TryAddScoped<ICheckLicenseRepository, CheckLicenseRepository>();
        service.TryAddScoped<IGetSellixCouponImplementation, GetSellixCouponImplementation>();
        service.TryAddScoped<IGetSellixCouponRepository, GetSellixCouponRepository>();
        service.TryAddScoped<IAdminCheckLicensesImplementation, AdminCheckLicensesImplementation>();
        service.TryAddScoped<IAdminCheckLicensesRepository, AdminCheckLicensesRepository>();
        service.TryAddScoped<IStaffLicenseImplementation, StaffLicenseImplementation>();
        service.TryAddScoped<IStaffLicenseRepository, StaffLicenseRepository>();
        service.TryAddScoped<IExtendLicensesImplementation, ExtendLicensesImplementation>();
        service.TryAddScoped<IExtendLicensesRepository, ExtendLicensesRepository>();
        service.TryAddScoped<IDatabaseBackupImplementation, DatabaseBackupImplementation>();
        service.TryAddScoped<IDatabaseBackupRepository, DatabaseBackupRepository>();
        service.TryAddScoped<IGrantLicenseImplementation, GrantLicenseImplementation>();
        service.TryAddScoped<IGrantLicenseRepository, GrantLicenseRepository>();
        service.TryAddScoped<IGptDiscordRepository, GptDiscordRepository>();
        service.TryAddScoped<IGptDiscordImplementation, GptDiscordImplementation>();
        service.TryAddScoped<IOnUserJoinRepository, OnUserJoinRepository>();
        service.TryAddScoped<IOnUserJoinImplementation, OnUserJoinImplementation>();
        service.TryAddScoped<IAlterLicenseImplementation, AlterLicenseImplementation>();
        service.TryAddScoped<IAlterLicenseRepository, AlterLicenseRepository>();
        service.TryAddSingleton<IDiscordServerMembersHandler, DiscordServerMembersHandler>();

        service.AddSingleton<AiModule>();

        service.AddLogging(x => x.AddLoggerConfig(configBuilder));

        service.TryAddSingleton<SlashCommandsHandler>();
        service.TryAddSingleton<CommandHandler>();
        service.TryAddSingleton<SlashCommandsBuilder>();
        service.TryAddSingleton<CommandService>();

        serviceProvider = service.BuildServiceProvider();

        var (socketClient, restClient) = await connectionHandler!.GetDiscordSocketRestClient(configBuilder["Discord:Token"] ?? string.Empty, true);
        service.AddSingleton(socketClient);
        service.TryAddScoped(_ => restClient);

        serviceProvider = service.BuildServiceProvider();

        CommandService commandService = connectionHandler!.GetCommandService();

        socketClient.Log += (logMessage) => LogAsync(logMessage, serviceProvider.GetRequiredService<ILogger<DiscordBot>>());
        restClient.Log += (logMessage) => LogAsync(logMessage, serviceProvider.GetRequiredService<ILogger<DiscordBot>>());

        var onMembersJoinLeaveEvent = serviceProvider.GetService<IDiscordServerMembersHandler>();

        var chandler = serviceProvider.GetService<CommandHandler>();
        var builder = serviceProvider.GetService<SlashCommandsBuilder>();

        socketClient.Ready += async () =>
        {
            await chandler?.InstallCommandsAsync(serviceProvider)!;
            await builder!.InstallSlashCommandsAsync();
            await onMembersJoinLeaveEvent!.Initialize(socketClient);
            await LogAsync(new LogMessage(LogSeverity.Info, "DiscordBot", "Bot is Ready"), serviceProvider.GetRequiredService<ILogger<DiscordBot>>());
        };

        var sHandler = serviceProvider.GetService<SlashCommandsHandler>();
        socketClient.SlashCommandExecuted += sHandler!.SlashCommandHandler;

        var onJoinEvent = serviceProvider.GetService<IOnUserJoinImplementation>();
        // Register event handler for UserJoined
        socketClient.UserJoined += async (user) =>
        {
            await onJoinEvent!.UserJoined(user);
            await onMembersJoinLeaveEvent!.Add(user);
        };

        socketClient.UserLeft += async (guild, user) =>
        {
            await onMembersJoinLeaveEvent!.Remove(guild, user);
        };

        await Task.Delay(Timeout.Infinite);
    }


    //AddScoped is used when you want to create a new instance of a service for each request within the scope.This means that if you request the same service multiple times within the same scope, you'll get the same instance.
    //AddTransient is used when you want to create a new instance of a service every time it is requested.This means that if you request the same service multiple times, you'll get a different instance each time.
    //AddSingleton is used when you want to create a single instance of a service for the lifetime of the application.This means that if you request the same service multiple times, you'll get the same instance each time.

    private static Task LogAsync(LogMessage logMessage, ILogger logger)
    {
        var severity = logMessage.Severity;

        logger.Log(severity switch
        {
            LogSeverity.Critical => LogLevel.Critical,
            LogSeverity.Error => LogLevel.Error,
            LogSeverity.Warning => LogLevel.Warning,
            LogSeverity.Info => LogLevel.Information,
            LogSeverity.Verbose => LogLevel.Trace,
            LogSeverity.Debug => LogLevel.Debug,
            _ => LogLevel.Information
        }, logMessage.ToString());

        Console.WriteLine(logMessage.ToString());
        return Task.CompletedTask;
    }
}