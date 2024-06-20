using Auth.Database.Contexts;
using Auth.Database.DbContextConfiguration;
using BoundBot.Connection.DiscordConnectionHandler.DiscordClientLibrary;
using Crosscutting.TLS.Configuration;
using Crosscutting.TransactionHandling;
using Database.Application.Implementation;
using Database.Application.Interface;
using Database.Infrastructure;
using DiscordBot.Application.Implementation;
using DiscordBot.Infrastructure;
using HostService.HostService;
using LoggingService.Components.SerilogConfiguration;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Quartz;
using System.Net;
using System.Security.Cryptography.X509Certificates;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("hostservice_appsettings.json");

builder.Configuration.AddEnvironmentVariables();

builder.Logging.AddLoggerConfig(builder.Configuration);

builder.Services.AddAuthDbContext(builder.Configuration);

//Dependency Injection
builder.Services.Scan(scan => scan
            .FromExecutingAssembly()
            .AddClasses()
            .AsMatchingInterface()
            .WithScopedLifetime());

//builder.Services.AddScoped<IUnitOfWork<IdentityDb>, UnitOfWork<IdentityDb>>();
builder.Services.TryAddScoped<IUnitOfWork<AuthDbContext>, UnitOfWork<AuthDbContext>>();
builder.Services.TryAddScoped<IDiscordBotCleanupRepository, DiscordBotCleanupRepository>();
builder.Services.TryAddScoped<IDiscordBotCleanupImplementation, DiscordBotCleanupImplementation>();
builder.Services.TryAddScoped<IMariaDbBackupImplementation, MariaDbBackupImplementation>();
builder.Services.TryAddScoped<IMariaDbBackupRepository, MariaDbBackupRepository>();
builder.Services.TryAddScoped<IDiscordConnectionHandler, DiscordConnectionHandler>();

builder.Services.AddQuartz(q =>
{
    var jobPurge = new JobKey("Purge");
    var jobBackup = new JobKey("Backup");

    q.AddJob<PurgeService>(opts =>
    {
        opts.WithIdentity(jobPurge);
    });

    q.AddJob<MariaDbBackup>(opts =>
    {
        opts.WithIdentity(jobBackup);
    });

    q.AddTrigger(opts =>
    {
        opts.ForJob(jobPurge);
        opts.WithIdentity("JobPurge-Trigger");
        opts.WithCronSchedule("0 0 0 ? * * *");
        //opts.WithSimpleSchedule(x=> {
        //    x.WithIntervalInSeconds(1);
        //    x.WithRepeatCount(0);
        //});
    });

    q.AddTrigger(opts =>
    {
        opts.ForJob(jobBackup);
        opts.WithIdentity("JobBackup-Trigger");
        opts.WithCronSchedule("0 0 0/6 ? * * *");
        //opts.WithSimpleSchedule(x =>
        //{
        //    x.WithIntervalInSeconds(1);
        //    x.WithRepeatCount(0);
        //});
    });

#pragma warning disable CS0618 // Type or member is obsolete
    q.UseMicrosoftDependencyInjectionJobFactory();
#pragma warning restore CS0618 // Type or member is obsolete
});

builder.Services.AddQuartzHostedService(opt =>
{
    opt.WaitForJobsToComplete = true;
});

//TLS
builder.WebHost.AddTLS(builder.Environment.IsDevelopment(), builder.Configuration);

if (!builder.Environment.IsDevelopment())
{
    var app = builder.Build();
    //app.MapGet("/", () => "Hello World!");
    app.Run();
}

