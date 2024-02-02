using BoundBot.Application.AdminCheckLicenses.Implementation;
using BoundBot.Application.AdminCheckLicenses.Interface;
using BoundBot.Application.CheckLicenses.Implementation;
using BoundBot.Application.CheckLicenses.Interface;
using BoundBot.Application.DatabaseBackup.Implementation;
using BoundBot.Application.DatabaseBackup.Interface;
using BoundBot.Application.EmbedBuilder.Implementation;
using BoundBot.Application.EmbedBuilder.Interface;
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
using BoundBot.Infrastructure.EmbedBuilder;
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

namespace BoundBot;

public class DiscordBot
{
    public static Task Main(string[] args) => new DiscordBot().MainAsync();

    async Task MainAsync()
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

        service.AddSingleton(connectionHandler!.GetDiscordSocketClient(configBuilder["Discord:Token"] ?? string.Empty));
        service.AddSingleton(connectionHandler.GetCommandService());
        service.AddScoped<IDomainRoleCheck, DomainRoleCheck>();
        service.AddScoped<IEmbedBuilderRepository, EmbedBuilderRepository>();
        service.AddScoped<IEmbedBuilderImplementation, EmbedBuilderImplementation>();
        service.AddScoped<IUpdateDiscordImplementation, UpdateDiscordImplementation>();
        service.AddScoped<IUpdateDiscordRepository, UpdateDiscordRepository>();
        service.AddScoped<IUpdateHwidImplementation, UpdateHwidImplementation>();
        service.AddScoped<IUpdateHwidRepository, UpdateHwidRepository>();
        service.AddScoped<ICheckLicenseImplementation, CheckLicenseImplementation>();
        service.AddScoped<ICheckLicenseRepository, CheckLicenseRepository>();
        service.AddScoped<IGetSellixCouponImplementation, GetSellixCouponImplementation>();
        service.AddScoped<IGetSellixCouponRepository, GetSellixCouponRepository>();
        service.AddScoped<IAdminCheckLicensesImplementation, AdminCheckLicensesImplementation>();
        service.AddScoped<IAdminCheckLicensesRepository, AdminCheckLicensesRepository>();
        service.AddScoped<IStaffLicenseImplementation, StaffLicenseImplementation>();
        service.AddScoped<IStaffLicenseRepository, StaffLicenseRepository>();
        service.AddScoped<IExtendLicensesImplementation, ExtendLicensesImplementation>();
        service.AddScoped<IExtendLicensesRepository, ExtendLicensesRepository>();
        service.AddScoped<IDatabaseBackupImplementation, DatabaseBackupImplementation>();
        service.AddScoped<IDatabaseBackupRepository, DatabaseBackupRepository>();
        service.AddScoped<IGrantLicenseImplementation, GrantLicenseImplementation>();
        service.AddScoped<IGrantLicenseRepository, GrantLicenseRepository>();
        service.AddScoped<IGptDiscordRepository, GptDiscordRepository>();
        service.AddScoped<IGptDiscordImplementation, GptDiscordImplementation>();
        service.AddScoped<IOnUserJoinRepository, OnUserJoinRepository>();
        service.AddScoped<IOnUserJoinImplementation, OnUserJoinImplementation>();
        service.AddScoped<IAlterLicenseImplementation, AlterLicenseImplementation>();
        service.AddScoped<IAlterLicenseRepository, AlterLicenseRepository>();
        service.TryAddSingleton<IDiscordServerMembersHandler, DiscordServerMembersHandler>();

        service.AddSingleton<AiModule>();

        service.AddLogging(x => x.AddLoggerConfig(configBuilder));

        service.AddSingleton<SlashCommandsHandler>();
        service.AddSingleton<CommandHandler>();
        service.AddSingleton<SlashCommandsBuilder>();
        service.AddSingleton<CommandService>();

        serviceProvider = service.BuildServiceProvider();

        DiscordSocketClient client = serviceProvider.GetService<DiscordSocketClient>()!;
        CommandService commandService = serviceProvider.GetRequiredService<CommandService>();

        client.Log += (logMessage) => LogAsync(logMessage, serviceProvider.GetRequiredService<ILogger<DiscordBot>>());

        var onMembersJoinLeaveEvent = serviceProvider.GetService<IDiscordServerMembersHandler>();

        var chandler = serviceProvider.GetService<CommandHandler>();
        await chandler?.InstallCommandsAsync(serviceProvider)!;

        var builder = serviceProvider.GetService<SlashCommandsBuilder>();
        
        client.Ready += async () =>
        {
            await builder!.Client_Ready();
            await onMembersJoinLeaveEvent!.Initialize(client);
        };

        var sHandler = serviceProvider.GetService<SlashCommandsHandler>();
        client.SlashCommandExecuted += sHandler!.SlashCommandHandler;

        var onJoinEvent = serviceProvider.GetService<IOnUserJoinImplementation>();
        // Register event handler for UserJoined
        client.UserJoined += async (user) =>
        {
            await onJoinEvent!.UserJoined(user);
            await onMembersJoinLeaveEvent!.Add(user);
        };

        client.UserLeft += async (guild, user) =>
        {
            await onMembersJoinLeaveEvent!.Remove(guild, user);
        };

        // Block this task until the program is closed.
        await Task.Delay(Timeout.Infinite);
    }


    //AddScoped is used when you want to create a new instance of a service for each request within the scope.This means that if you request the same service multiple times within the same scope, you'll get the same instance.
    //AddTransient is used when you want to create a new instance of a service every time it is requested.This means that if you request the same service multiple times, you'll get a different instance each time.
    //AddSingleton is used when you want to create a single instance of a service for the lifetime of the application.This means that if you request the same service multiple times, you'll get the same instance each time.

    private Task LogAsync(LogMessage logMessage, ILogger logger)
    {
        logger.LogInformation(logMessage.ToString());
        Console.WriteLine(logMessage.ToString());
        return Task.CompletedTask;
    }
}