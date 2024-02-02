using Auth.Database;
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

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("api_discordbot_appsettings.json");

//Docker
builder.Configuration.AddEnvironmentVariables();

builder.Logging.AddLoggerConfig(builder.Configuration);

builder.Services.AddAuthDbContext(builder.Configuration);

//Dependency Injection
builder.Services.Scan(a => a.FromCallingAssembly().AddClasses().AsMatchingInterface().WithScopedLifetime());
//builder.Services.AddScoped<IUnitOfWork<IdentityDb>, UnitOfWork<IdentityDb>>();
builder.Services.AddScoped<IUnitOfWork<AuthDbContext>, UnitOfWork<AuthDbContext>>();
builder.Services.AddScoped<IDiscordBotQueryRepository, DiscordBotQueryRepository>();
builder.Services.AddScoped<IDiscordBotQueryImplementation, DiscordBotQueryImplementation>();
builder.Services.AddScoped<IDiscordBotCommandRepository, DiscordBotCommandRepository>();
builder.Services.AddScoped<IDiscordBotCommandImplementation, DiscordBotCommandImplementation>();
builder.Services.AddScoped<IDiscordConnectionHandler, DiscordConnectionHandler>();
builder.Services.AddScoped<IDiscordServerMembersHandler, DiscordServerMembersHandler>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//JWT
builder.Services.AddJwtConfiguration(builder.Configuration);

//Policies
builder.Services.AddClaimPolicyConfiguration();

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