using Admin.Application.Implementation.AlterLicense;
using Admin.Application.Implementation.ExtendLicense;
using Admin.Application.Implementation.GrantLicense;
using Admin.Application.Interface.AlterLicense;
using Admin.Application.Interface.ExtendLicense;
using Admin.Application.Interface.GrantLicense;
using Admin.Infrastructure.ExtendLicense;
using Admin.Infrastructure.GrantLicense;
using Admin.Infrastructure.IAlterLicense;
using Auth.Database;
using Auth.Database.DbContextConfiguration;
using BoundBot.Connection.DiscordConnectionHandler.DiscordClientLibrary;
using Crosscutting.Configuration.AuthPolicyConfiguration;
using Crosscutting.Configuration.JwtConfiguration;
using Crosscutting.TransactionHandling;
using Database.Application.Implementation;
using Database.Application.Interface;
using Database.Infrastructure;
using LoggingService.Components.SerilogConfiguration;
using Microsoft.AspNetCore.DataProtection;
using System.Net;
using System.Security.Cryptography.X509Certificates;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//Docker
builder.Configuration.AddEnvironmentVariables();

builder.Logging.AddLoggerConfig(builder.Configuration);

builder.Services.AddAuthDbContext(builder.Configuration);

//Dependency Injection
builder.Services.Scan(a => a.FromCallingAssembly().AddClasses().AsMatchingInterface().WithScopedLifetime());
builder.Services.AddScoped<IUnitOfWork<AuthDbContext>, UnitOfWork<AuthDbContext>>();
builder.Services.AddScoped<IAdminExtendLicensesRepository, AdminExtendLicensesRepository>();
builder.Services.AddScoped<IAdminExtendLicensesImplementation, AdminExtendLicensesImplementation>();
builder.Services.AddScoped<IMariaDbBackupRepository, MariaDbBackupRepository>();
builder.Services.AddScoped<IMariaDbBackupImplementation, MariaDbBackupImplementation>();
builder.Services.AddScoped<IDiscordConnectionHandler, DiscordConnectionHandler>();
builder.Services.AddScoped<IAdminGrantLicenseImplementation, AdminGrantLicenseImplementation>();
builder.Services.AddScoped<IAdminGrantLicenseRepository, AdminGrantLicenseRepository>();
builder.Services.AddScoped<IAlterLicenseImplementation, AlterLicenseImplementation>();
builder.Services.AddScoped<IAlterLicenseRepository, AlterLicenseRepository>();

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
