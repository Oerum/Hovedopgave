using Auth.Database.Contexts;
using Auth.Database.DbContextConfiguration;
using BoundBot.Connection.DiscordConnectionHandler.DiscordClientLibrary;
using Crosscutting.Configuration.AuthPolicyConfiguration;
using Crosscutting.Configuration.JwtConfiguration;
using Crosscutting.TLS.Configuration;
using Crosscutting.TransactionHandling;
using gpt.application.Implementation;
using gpt.application.Interface;
using Gpt.Infrastructure;
using LoggingService.Components.SerilogConfiguration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Net;
using System.Security.Cryptography.X509Certificates;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

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
builder.Services.TryAddScoped<IDiscordConnectionHandler, DiscordConnectionHandler>();
builder.Services.TryAddScoped<IGptImplementation, GptImplementation>();
builder.Services.TryAddScoped<IGptRepository, GptRepository>();


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//JWT
builder.Services.AddJwtConfiguration(builder.Configuration);

//Policies
builder.Services.AddClaimPolicyConfiguration();

//OpenAI
builder.Services.AddOpenAi(settings =>
{
    settings.ApiKey = builder.Configuration["OpenAI:Key"];
    settings.OrganizationName = builder.Configuration["OpenAI:Org"];
});

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
