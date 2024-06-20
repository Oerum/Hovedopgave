using Admin.Application.Implementation.AlterLicense;
using Admin.Application.Implementation.ExtendLicense;
using Admin.Application.Implementation.GrantLicense;
using Admin.Application.Interface.AlterLicense;
using Admin.Application.Interface.ExtendLicense;
using Admin.Application.Interface.GrantLicense;
using Admin.Infrastructure.ExtendLicense;
using Admin.Infrastructure.GrantLicense;
using Admin.Infrastructure.IAlterLicense;
using Auth.Database.Contexts;
using Auth.Database.DbContextConfiguration;
using BoundBot.Connection.DiscordConnectionHandler.DiscordClientLibrary;
using Crosscutting.Configuration.AuthPolicyConfiguration;
using Crosscutting.Configuration.JwtConfiguration;
using Crosscutting.TLS.Configuration;
using Crosscutting.TransactionHandling;
using Database.Application.Implementation;
using Database.Application.Interface;
using Database.Infrastructure;
using LoggingService.Components.SerilogConfiguration;
using Microsoft.AspNetCore.DataProtection;
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

builder.Services.TryAddScoped<IUnitOfWork<AuthDbContext>, UnitOfWork<AuthDbContext>>();
builder.Services.TryAddScoped<IAdminExtendLicensesRepository, AdminExtendLicensesRepository>();
builder.Services.TryAddScoped<IAdminExtendLicensesImplementation, AdminExtendLicensesImplementation>();
builder.Services.TryAddScoped<IMariaDbBackupRepository, MariaDbBackupRepository>();
builder.Services.TryAddScoped<IMariaDbBackupImplementation, MariaDbBackupImplementation>();
builder.Services.TryAddScoped<IDiscordConnectionHandler, DiscordConnectionHandler>();
builder.Services.TryAddScoped<IAdminGrantLicenseImplementation, AdminGrantLicenseImplementation>();
builder.Services.TryAddScoped<IAdminGrantLicenseRepository, AdminGrantLicenseRepository>();
builder.Services.TryAddScoped<IAlterLicenseImplementation, AlterLicenseImplementation>();
builder.Services.TryAddScoped<IAlterLicenseRepository, AlterLicenseRepository>();

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
