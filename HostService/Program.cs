using Auth.Database;
using Auth.Database.DbContextConfiguration;
using BoundBot.Connection.DiscordConnectionHandler.DiscordClientLibrary;
using Crosscutting.TransactionHandling;
using Database.Application.Implementation;
using Database.Application.Interface;
using Database.Infrastructure;
using DiscordBot.Application.Implementation;
using DiscordBot.Infrastructure;
using HostService.HostService;
using LoggingService.Components.SerilogConfiguration;
using Microsoft.AspNetCore.DataProtection;
using Quartz;
using System.Net;
using System.Security.Cryptography.X509Certificates;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("hostservice_appsettings.json");

builder.Configuration.AddEnvironmentVariables();

builder.Logging.AddLoggerConfig(builder.Configuration);

builder.Services.AddAuthDbContext(builder.Configuration);

//Dependency Injection
builder.Services.Scan(a => a.FromCallingAssembly().AddClasses().AsMatchingInterface().WithScopedLifetime());
//builder.Services.AddScoped<IUnitOfWork<IdentityDb>, UnitOfWork<IdentityDb>>();
builder.Services.AddScoped<IUnitOfWork<AuthDbContext>, UnitOfWork<AuthDbContext>>();
builder.Services.AddScoped<IDiscordBotCleanupRepository, DiscordBotCleanupRepository>();
builder.Services.AddScoped<IDiscordBotCleanupImplementation, DiscordBotCleanupImplementation>();
builder.Services.AddScoped<IMariaDbBackupImplementation, MariaDbBackupImplementation>();
builder.Services.AddScoped<IMariaDbBackupRepository, MariaDbBackupRepository>();
builder.Services.AddScoped<IDiscordConnectionHandler, DiscordConnectionHandler>();

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
var certificate = new X509Certificate2(builder.Configuration["Cert:Gateway"]!, builder.Configuration["Cert:Gateway:Password"]);
//builder.Services.AddDataProtection().ProtectKeysWithCertificate(certificate);

builder.WebHost.UseKestrel(options =>
{
    var appServices = options.ApplicationServices;

    options.Listen(IPAddress.Any, 80, listenOptions =>
    {
        listenOptions.UseConnectionLogging();
    });

    options.Listen(IPAddress.Any, 443, listenOptions =>
    {
        listenOptions.UseHttps(certificate);
        listenOptions.UseConnectionLogging();
    });
});

if (!builder.Environment.IsDevelopment())
{
    var app = builder.Build();
    //app.MapGet("/", () => "Hello World!");
    app.Run();
}

