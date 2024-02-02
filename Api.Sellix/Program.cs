using Auth.Database;
using Auth.Database.DbContextConfiguration;
using BrokersService.MassTransitServiceCollection;
using Crosscutting.Configuration.AuthPolicyConfiguration;
using Crosscutting.Configuration.JwtConfiguration;
using Crosscutting.TransactionHandling;
using LoggingService.Components.SerilogConfiguration;
using Microsoft.AspNetCore.DataProtection;
using Sellix.Application.Implementation;
using Sellix.Application.Interfaces;
using Sellix.Components;
using Sellix.Infrastructure;
using System.Net;
using System.Security.Cryptography.X509Certificates;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("api_sellix_appsettings.json");

//Docker
builder.Configuration.AddEnvironmentVariables();

builder.Logging.AddLoggerConfig(builder.Configuration);

builder.Services.AddAuthDbContext(builder.Configuration);

//Dependency Injection
builder.Services.Scan(a => a.FromExecutingAssembly().AddClasses().AsMatchingInterface().WithScopedLifetime());
builder.Services.AddScoped<IUnitOfWork<AuthDbContext>, UnitOfWork<AuthDbContext>>();
builder.Services.AddScoped<ISellixCouponCreateRepository, SellixCouponCreateRepository>();
builder.Services.AddScoped<ISellixCouponCreateImplementation, SellixCouponCreateImplementation>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//JWT
builder.Services.AddJwtConfiguration(builder.Configuration);

//Policies
builder.Services.AddClaimPolicyConfiguration();

//Kafka
builder.Services.AddMassTransitWithRabbitMqAndKafka(builder.Configuration, true);

//SellixAPI
builder.Services.SellixApi(builder.Configuration);

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