using System.Net;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using Auth.Application.Implementation;
using Auth.Application.Interface;
using Auth.Database.Contexts;
using Auth.Database.DbContextConfiguration;
using Auth.Domain;
using Auth.Infrastructure;
using Crosscutting.Configuration.AuthPolicyConfiguration;
using Crosscutting.Configuration.JwtConfiguration;
using Crosscutting.TLS.Configuration;
using Crosscutting.TransactionHandling;
using LoggingService.Components.SerilogConfiguration;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

var builder = WebApplication.CreateBuilder(args);

//Docker
builder.Configuration.AddEnvironmentVariables();

builder.Logging.AddLoggerConfig(builder.Configuration);

//DbContext
builder.Services.AddAuthDbContext(builder.Configuration);
builder.Services.AddOAuthContext(builder.Configuration);

//Dependency Injection
//builder.Services.AddScoped<IUnitOfWork<IdentityDb>, UnitOfWork<IdentityDb>>();
builder.Services.AddScoped<IUnitOfWork<AuthDbContext>, UnitOfWork<AuthDbContext>>();
builder.Services.Scan(scan => scan
            .FromExecutingAssembly()
            .AddClasses()
            .AsMatchingInterface()
            .WithScopedLifetime());

builder.Services.TryAddScoped<IAuthRepository, AuthRepository>();
builder.Services.TryAddScoped<IAuthImplementation, AuthImplementation>();
builder.Services.TryAddScoped<IAuthDomain, AuthDomain>();
builder.Services.TryAddScoped<IUnitOfWork<DiscordOAuthContext>, UnitOfWork<DiscordOAuthContext>>();


// Add services to the container.

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
