using Auth.Database;
using Auth.Database.DbContextConfiguration;
using BrokersService.MassTransitServiceCollection;
using Crosscutting.TransactionHandling;
using DiscordBot.Application.Interface;
using DiscordBot.Infrastructure;
using LoggingService.Components.SerilogConfiguration;
using Microsoft.AspNetCore.DataProtection;
using Sellix.Application.Interfaces;
using Sellix.Infrastructure;
using Serilog;
using System.Net;
using System.Security.Cryptography.X509Certificates;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("brokerservice_appsettings.json");

builder.Configuration.AddEnvironmentVariables();

builder.Logging.AddLoggerConfig(builder.Configuration);

builder.Services.AddAuthDbContext(builder.Configuration);

builder.Services.AddScoped<IUnitOfWork<AuthDbContext>, UnitOfWork<AuthDbContext>>();
builder.Services.AddScoped<IDiscordBotNotificationRepository, DiscordBotNotificationRepository>();
builder.Services.AddScoped<ISellixGatewayBuyHandlerRepository, SellixGatewayBuyHandlerRepository>();

//Kafka
builder.Services.AddMassTransitWithRabbitMqAndKafka(builder.Configuration, true);

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

var app = builder.Build();

//app.MapGet("/", () => "Hello World!");

app.Run();



