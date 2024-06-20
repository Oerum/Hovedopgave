using System.Net;
using System.Security.Cryptography.X509Certificates;
using Crosscutting.TLS.Configuration;
using LoggingService.Components.SerilogConfiguration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//Docker
builder.Configuration.AddEnvironmentVariables();

builder.Logging.AddLoggerConfig(builder.Configuration);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHealthChecks();
builder.Services.AddHttpForwarder();

//YARP
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

//TLS
builder.WebHost.AddTLS(builder.Environment.IsDevelopment(), builder.Configuration);

var app = builder.Build();

app.MapHealthChecks("/health");

app.MapReverseProxy();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseHttpsRedirection();
    app.UseForwardedHeaders();
}

app.Logger.LogInformation("ApiGateway Environment: " + app.Environment.EnvironmentName);

app.UseAuthorization();

app.MapControllers();

app.Run();
