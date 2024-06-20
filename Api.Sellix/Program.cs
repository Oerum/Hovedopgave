using Auth.Database.Contexts;
using Auth.Database.DbContextConfiguration;
using BrokersService.MassTransitServiceCollection;
using Crosscutting.Configuration.AuthPolicyConfiguration;
using Crosscutting.Configuration.JwtConfiguration;
using Crosscutting.TLS.Configuration;
using Crosscutting.TransactionHandling;
using LoggingService.Components.SerilogConfiguration;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.DependencyInjection.Extensions;
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
builder.Services.Scan(scan => scan
            .FromExecutingAssembly()
            .AddClasses()
            .AsMatchingInterface()
            .WithScopedLifetime());

builder.Services.TryAddScoped<IUnitOfWork<AuthDbContext>, UnitOfWork<AuthDbContext>>();
builder.Services.TryAddScoped<ISellixCouponCreateRepository, SellixCouponCreateRepository>();
builder.Services.TryAddScoped<ISellixCouponCreateImplementation, SellixCouponCreateImplementation>();

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