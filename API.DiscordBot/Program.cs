using Auth.Database.DbContextConfiguration;
using BoundBot.Connection.DiscordConnectionHandler.DiscordClientLibrary;
using BrokersService.MassTransitServiceCollection;
using Crosscutting.Configuration.AuthPolicyConfiguration;
using Crosscutting.Configuration.JwtConfiguration;
using Crosscutting.TransactionHandling;
using Database.Application.Interface;
using DiscordBot.Application.Implementation;
using DiscordBot.Application.Interface;
using DiscordBot.Infrastructure;
using LoggingService.Components.SerilogConfiguration;
using Microsoft.AspNetCore.DataProtection;
using System.Security.Cryptography.X509Certificates;
using BoundBot.Components.Members;
using System.Net;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Crosscutting.TLS.Configuration;
using Auth.Database.Contexts;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("api_discordbot_appsettings.json");

//Docker
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
builder.Services.TryAddScoped<IDiscordBotQueryRepository, DiscordBotQueryRepository>();
builder.Services.TryAddScoped<IDiscordBotQueryImplementation, DiscordBotQueryImplementation>();
builder.Services.TryAddScoped<IDiscordBotCommandRepository, DiscordBotCommandRepository>();
builder.Services.TryAddScoped<IDiscordBotCommandImplementation, DiscordBotCommandImplementation>();
builder.Services.TryAddScoped<IDiscordConnectionHandler, DiscordConnectionHandler>();
builder.Services.TryAddScoped<IDiscordServerMembersHandler, DiscordServerMembersHandler>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//JWT
builder.Services.AddJwtConfiguration(builder.Configuration);

//Policies
builder.Services.AddClaimPolicyConfiguration();

//TLS
builder.WebHost.AddTLS(builder.Environment.IsDevelopment(), builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();
app.UseForwardedHeaders();

app.UseAuthorization();

app.MapControllers();

app.Run();