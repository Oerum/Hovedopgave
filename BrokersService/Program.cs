using Auth.Database.Contexts;
using Auth.Database.DbContextConfiguration;
using BrokersService.MassTransitServiceCollection;
using Crosscutting.TLS.Configuration;
using Crosscutting.TransactionHandling;
using DiscordBot.Application.Interface;
using DiscordBot.Infrastructure;
using LoggingService.Components.SerilogConfiguration;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.DependencyInjection.Extensions;
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

builder.Services.Scan(scan => scan
            .FromExecutingAssembly()
            .AddClasses()
            .AsMatchingInterface()
            .WithScopedLifetime());

builder.Services.TryAddScoped<IUnitOfWork<AuthDbContext>, UnitOfWork<AuthDbContext>>();
builder.Services.TryAddScoped<IDiscordBotNotificationRepository, DiscordBotNotificationRepository>();
builder.Services.TryAddScoped<ISellixGatewayBuyHandlerRepository, SellixGatewayBuyHandlerRepository>();

//Kafka
builder.Services.AddMassTransitWithRabbitMqAndKafka(builder.Configuration, true);

//TLS
builder.WebHost.AddTLS(builder.Environment.IsDevelopment(), builder.Configuration);

var app = builder.Build();

//app.MapGet("/", () => "Hello World!");

app.Run();



